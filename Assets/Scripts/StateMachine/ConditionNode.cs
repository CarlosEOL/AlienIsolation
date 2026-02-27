using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Condition")]
    public class ConditionNode : Node
    {
        [SerializeField] Node[] children;
        
        public override NodeStatus Execute(NPC npc)
        {
            
            
            return NodeStatus.Failure;
        }
    }
}