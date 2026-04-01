
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
            switch (npc.currentGoals)
            {
                case IStateAndGoals.NPCGoals.Idle:
                    npc.currentState = IStateAndGoals.NPCState.Idle;
                    IdleNode.Execute(npc);
                    break;
                
                case IStateAndGoals.NPCGoals.FindAndKill:
                    npc.currentState = IStateAndGoals.NPCState.Hunt;
                    HuntNode.Execute(npc);
                    Debug.Log($"{npc.name} has Started to HUNT!");
                    break;
                
                case IStateAndGoals.NPCGoals.Lead:
                    npc.currentState = IStateAndGoals.NPCState.Hunt;
                    HuntNode.Execute(npc);
                    break;
                
                case IStateAndGoals.NPCGoals.Protect:
                    npc.currentState = IStateAndGoals.NPCState.Pursue;
                    PursueNode.Execute(npc);
                    break;
                
                case IStateAndGoals.NPCGoals.Wander:
                    npc.currentState = IStateAndGoals.NPCState.Wander;
                    WanderNode.Execute(npc);
                    break;
            }
            
            return NodeStatus.Running;
        }
    }
}