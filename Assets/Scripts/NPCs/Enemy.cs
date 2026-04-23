
using StateMachine;
using UnityEngine;

namespace NPCs
{
    public class Enemy : NPC
    {
        GameObject currentTarget;
        
        protected override void OnTargetDetected(Transform detectedTarget)
        {
            currentTarget = detectedTarget.gameObject;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            float viewAngle = 20f;
            float viewDistance = 20f;
    
            // Draw the two outer edges of the cone
            Vector3 leftBoundary = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up) * transform.forward;
            Vector3 rightBoundary = Quaternion.AngleAxis(viewAngle / 2, Vector3.up) * transform.forward;

            Gizmos.DrawRay(pos, leftBoundary * viewDistance);
            Gizmos.DrawRay(pos, rightBoundary * viewDistance);
            
#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1, 1, 0, 0.1f);
            UnityEditor.Handles.DrawSolidArc(pos, Vector3.up, leftBoundary, viewAngle, viewDistance);
#endif
        }

        public override void Attack()
        {
            base.Attack();
            
            Debug.Log($"currentTarget: {currentTarget?.name ?? "NULL"} | npc.Target: {Target?.name ?? "NULL"}");
            
            if (currentTarget == null && Target != null)
                currentTarget = Target.gameObject;
    
            if (currentTarget == null || !currentTarget.activeInHierarchy)
            {
                currentTarget = null;
                Target = null;
                return;
            }
    
            Debug.Log($"Distance: {Vector3.Distance(transform.position, currentTarget.transform.position)} | Target: {currentTarget.name}");

            
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1.5f)
            {
                Destroy(currentTarget.gameObject);
                Debug.Log($"Destroyed target: {currentTarget.name}");
                currentTarget = null;
                Target = null;
                currentState = IStateAndGoals.NPCState.Hunt;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Always prioritize player
            if (other.gameObject.CompareTag("Player"))
            {
                currentTarget = other.gameObject;
                return;
            }

            // Only take friendly if no target yet
            if (currentTarget == null && other.gameObject.CompareTag("Friendly"))
                currentTarget = other.gameObject;
        }
    }

}
