using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Wonder")]
    public class WonderAction : Node, IAction
    {
        public float speed;
        public override NodeStatus Execute(NPC npc) 
        {
            if (npc.target == null) return NodeStatus.Failure;
            
            if (npc.target != null) 
            {
                npc.agent.speed = speed;
                npc.agent.SetDestination(npc.target.position);
                //npc.Animator.SetBool("IsChasing", true);
            }
            
            return NodeStatus.Running;
        }
    }
}