using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Condition")]
    public class ConditionNode : Node
    {
        public string npcMethodCallback;

        public override NodeStatus Execute(NPC npc)
        {
            // Search the NPC script for a method with this name
            var method = npc.GetType().GetMethod(npcMethodCallback);
            bool result = (bool)method.Invoke(npc, null);
    
            return result ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}