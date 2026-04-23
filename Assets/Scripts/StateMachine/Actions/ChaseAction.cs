using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Chase")]
    public class ChaseAction : Node
    {
        public override NodeStatus Execute(NPC npc)
        {
            if (npc.Target == null || !npc.Target.gameObject.activeInHierarchy) 
                return NodeStatus.Failure;
            
            if (npc.CheckIsInTargetRange())
            {
                npc.currentState = IStateAndGoals.NPCState.Attack;
                return NodeStatus.Success;
            }

            if (npc.HasTargetInSight())
            {
                npc.agent.SetDestination(npc.Target.position);
                return NodeStatus.Running;
            }

            return NodeStatus.Failure;
        }
    }
}