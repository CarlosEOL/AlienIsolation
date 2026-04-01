using UnityEngine;

namespace NPCs
{
    public class Enemy : NPC
    {
        
        
        
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
