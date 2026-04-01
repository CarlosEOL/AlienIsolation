using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Chase")]
    public class ChaseAction : Node
    {
        public override NodeStatus Execute(NPC npc)
        {
            if (npc.Target.CompareTag("Friendly"))
            {
                if (npc.HasTargetInsight())
                {
                    npc.agent.SetDestination(npc.Target.position);

                    if (npc.CheckIsInTargetRange())
                    {
                        npc.currentState = IStateAndGoals.NPCState.Attack;
                        return NodeStatus.Success;
                    }
                
                    return NodeStatus.Running;
                }
            }
            
            if (npc.Target.CompareTag("Player"))
            {
                if (npc.HasTargetInsight())
                {
                    npc.agent.SetDestination(npc.Target.position);

                    if (npc.CheckIsInTargetRange())
                    {
                        npc.currentState = IStateAndGoals.NPCState.Attack;
                        return NodeStatus.Success;
                    }
                
                    return NodeStatus.Running;
                }
            }
            
            if (npc.Target.CompareTag("Enemy"))
            {
                if (npc.HasTargetInsight())
                {
                    npc.agent.SetDestination(npc.Target.position);

                    if (npc.CheckIsInTargetRange())
                    {
                        npc.currentState = IStateAndGoals.NPCState.Attack;
                        return NodeStatus.Success;
                    }
                
                    return NodeStatus.Running;
                }
            }
            
            return NodeStatus.Failure;
        }
    }
}