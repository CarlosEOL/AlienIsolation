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
        [SerializeField] private float runRadius = 10f;    // beyond this = run
        [SerializeField] private float flockRadius = 5f;   // within this = match velocity
        [SerializeField] private float stopRadius = 1.5f;  // within this = stop

        public override NodeStatus Execute(NPC npc)
        {
            if (npc.Target == null) return NodeStatus.Failure;

            Vector3 targetPosition = npc.Target.position;
            Vector3 targetVelocity = npc.targetLinearVelocity;
            float distanceToTarget = Vector3.Distance(npc.transform.position, targetPosition);

            // --- Zone 3: Too close, stop immediately ---
            if (distanceToTarget < stopRadius)
            {
               
                npc.agent.speed = 0f;
                npc.agent.SetDestination(npc.transform.position);
                npc.IsRunning = false;
                
                return NodeStatus.Running;
            }

            // --- Zone 2: Flock zone, match velocity ---
            if (distanceToTarget < flockRadius)
            {
                npc.IsRunning = false;
                Vector3 predictedDestination = targetPosition + (targetVelocity * lookaheadTime) + npc.CalculateSepereationOffset();

                if (Vector3.Distance(npc.agent.destination, predictedDestination) > destinationUpdateThreshold)
                    npc.agent.SetDestination(predictedDestination);

                // Smoothly match target speed
                npc.agent.speed = Mathf.Lerp(npc.agent.speed, targetVelocity.magnitude, Time.deltaTime * speedSmoothing);
                return NodeStatus.Running;
            }

            // --- Zone 1: Run to target ---
            if (distanceToTarget > runRadius)
            {
                npc.IsRunning = true;
                npc.agent.speed = npc.runSpeed;
            }
            else
            {
                // Between flockRadius and runRadius - walk
                npc.IsRunning = false;
                npc.agent.speed = npc.walkSpeed;
            }

            Vector3 destination = targetPosition + npc.CalculateSepereationOffset();
            if (Vector3.Distance(npc.agent.destination, destination) > destinationUpdateThreshold)
                npc.agent.SetDestination(destination);

            return NodeStatus.Running;
        }
    }
}