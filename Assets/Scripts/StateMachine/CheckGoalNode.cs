
using System.Collections.Generic;
using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/CheckGoalsNode", order = 0)]
    public class CheckGoalNode : Node
    {
        [SerializeField] public Node IdleNode;
        [SerializeField] public Node HuntNode;
        [SerializeField] public Node PursueNode;
        [SerializeField] public Node WanderNode;
        public override NodeStatus Execute(NPC npc)
        {
            IStateAndGoals.NPCGoals currentGoals = npc.currentGoals;
            
            switch (currentGoals)
            {
                case IStateAndGoals.NPCGoals.Idle:
                    npc.currentState = IStateAndGoals.NPCState.Idle;
                    return IdleNode.Execute(npc);
                
                case IStateAndGoals.NPCGoals.FindAndKill:
                    npc.currentState = IStateAndGoals.NPCState.Hunt;
                    return HuntNode.Execute(npc);
                
                case IStateAndGoals.NPCGoals.Lead:
                    npc.currentState = IStateAndGoals.NPCState.Hunt;
                    return HuntNode.Execute(npc);
                
                case IStateAndGoals.NPCGoals.Protect:
                    npc.currentState = IStateAndGoals.NPCState.Pursue;
                    return PursueNode.Execute(npc);
                
                case IStateAndGoals.NPCGoals.Wander:
                    npc.currentState = IStateAndGoals.NPCState.Wander;
                    return WanderNode.Execute(npc);
            }
            
            return NodeStatus.Failure;
        }
    }
}