using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Flock")]
    public class FlockAction : Node
    {
        [SerializeField] private float lookaheadTime = 0.5f;
        [SerializeField] private float speedSmoothing = 3f;
        [SerializeField] private float destinationUpdateThreshold = 0.5f;

        [Header("Zones")]
        [SerializeField] private float runRadius = 10f;
        [SerializeField] private float flockRadius = 5f;
        [SerializeField] private float flockExitRadius = 6f;    // must go further out to exit flock zone
        [SerializeField] private float stopRadius = 1.5f;
        [SerializeField] private float stopExitRadius = 2f;     // must go further out to exit stop zone

        private bool _isFlocking = false;
        private bool _isStopped = false;

        public override NodeStatus Execute(NPC npc)
        {
            if (npc.Target == null) return NodeStatus.Failure;

            Vector3 targetPosition = npc.Target.position;
            Vector3 targetVelocity = npc.targetLinearVelocity;
            float distanceToTarget = Vector3.Distance(npc.transform.position, targetPosition);

            // Update zone states with hysteresis
            if (!_isStopped && distanceToTarget < stopRadius)       _isStopped = true;
            if (_isStopped && distanceToTarget > stopExitRadius)    _isStopped = false;

            if (!_isFlocking && distanceToTarget < flockRadius)     _isFlocking = true;
            if (_isFlocking && distanceToTarget > flockExitRadius)  _isFlocking = false;

            // --- Zone 3: Stop ---
            if (_isStopped)
            {
                npc.agent.speed = 0f;
                npc.agent.SetDestination(npc.transform.position);
                npc.IsRunning = false;
                return NodeStatus.Running;
            }

            // --- Zone 2: Flock ---
            if (_isFlocking)
            {
                npc.IsRunning = false;
                Vector3 predictedDestination = targetPosition + (targetVelocity * lookaheadTime) + npc.CalculateSeparationOffset();

                if (Vector3.Distance(npc.agent.destination, predictedDestination) > destinationUpdateThreshold)
                    npc.agent.SetDestination(predictedDestination);

                npc.agent.speed = Mathf.Lerp(npc.agent.speed, targetVelocity.magnitude, Time.deltaTime * speedSmoothing);
                return NodeStatus.Running;
            }

            // --- Zone 1: Run or walk ---
            if (distanceToTarget > runRadius)
            {
                npc.IsRunning = true;
                npc.agent.speed = npc.runSpeed;
            }
            else
            {
                npc.IsRunning = false;
                npc.agent.speed = npc.walkSpeed;
            }

            Vector3 destination = targetPosition + npc.CalculateSeparationOffset();
            if (Vector3.Distance(npc.agent.destination, destination) > destinationUpdateThreshold)
                npc.agent.SetDestination(destination);

            return NodeStatus.Running;
        }
    }
}