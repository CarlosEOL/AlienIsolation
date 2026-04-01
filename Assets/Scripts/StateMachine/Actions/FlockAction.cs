using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Flock")]
    public class FlockAction : Node
    {
        [SerializeField] private float lookaheadTime = 0.5f;
        
        [SerializeField] private float arrivalRadius = 1f;
        [SerializeField] private float speedSmoothing = 3f;
        [SerializeField] private float destinationUpdateThreshold = 0.5f;
        [SerializeField] private GameObject healthItemPrefab;

        public override NodeStatus Execute(NPC npc)
        {
            Debug.Log($"Currently Called Flock by: {npc.name}");
            if (npc.Target ==null) return NodeStatus.Failure;
            
            Vector3 targetPosition = npc.Target.position;
            Vector3 targetVelocity = npc.TargetLinearVelocity;

            // Predictions
            Vector3 predictedDestination = targetPosition + (targetVelocity * lookaheadTime) + npc.CalculateSepereationOffset();

            // Goto with speed clamp
            float distanceToTarget = Vector3.Distance(npc.transform.position, targetPosition);

            if (distanceToTarget > arrivalRadius)
            {
                if (Vector3.Distance(npc.agent.destination, predictedDestination) > destinationUpdateThreshold)
                    npc.agent.SetDestination(predictedDestination);
            }
            else
            {
                // Close enough - idle match, stop drifting
                npc.agent.SetDestination(npc.transform.position);
                npc.agent.speed = Mathf.Lerp(npc.agent.speed, 0f, Time.deltaTime * speedSmoothing);
                
                // Match player speed
                float targetSpeed = targetVelocity.magnitude;
                npc.agent.speed = Mathf.Lerp(npc.agent.speed, targetSpeed, Time.deltaTime * speedSmoothing);
            }

            return NodeStatus.Running;
        }
    }
}