using UnityEngine;

namespace NPCs
{
    public class Enemy : NPC
    {
        public bool IsPlayerInCone(Transform npc, Transform player, float maxDist, float maxAngle)
        {
            Vector3 directionToPlayer = (player.position - npc.position).normalized;
            float distance = Vector3.Distance(npc.position, player.position);

            if (distance < maxDist)
            {
                // Check if player is within the FOV angle
                if (Vector3.Angle(npc.forward, directionToPlayer) < maxAngle / 2)
                {
                    // Raycast to check for walls/obstacles
                    if (!Physics.Linecast(npc.position, player.position, layerMask: player.gameObject.layer))
                    {
                        Debug.Log("I SEE YOU!");
                        return true; // Detected!
                    }
                }
            }
            return false;
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

            // Optional: Draw a wire arc to connect the rays
#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1, 1, 0, 0.1f); // Semi-transparent yellow
            UnityEditor.Handles.DrawSolidArc(pos, Vector3.up, leftBoundary, viewAngle, viewDistance);
#endif
        }
    }

}
