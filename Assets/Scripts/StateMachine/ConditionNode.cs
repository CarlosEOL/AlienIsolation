using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Condition")]
    public class ConditionNode : Node
    {
        public string npcMethodCallback;
        private System.Reflection.MethodInfo _cachedMethod;

        [SerializeField] private Node TrueNode;
        [SerializeField] private Node FalseNode;

        public override NodeStatus Execute(NPC npc)
        {
            NodeStatus status;
            
            // Search the NPC script for a method with this name
            if (_cachedMethod == null)
                _cachedMethod = npc.GetType().GetMethod(npcMethodCallback);

            if (_cachedMethod == null) return NodeStatus.Failure;

            bool result = (bool)_cachedMethod.Invoke(npc, null);
            Debug.Log("Condition Result: " + result);
            
            if (result)
            {
                status = TrueNode.Execute(npc);
            }
            else
            {
                status = FalseNode.Execute(npc);
            }
            
            return status == NodeStatus.Running ? NodeStatus.Running : NodeStatus.Success;
        }
    }
}