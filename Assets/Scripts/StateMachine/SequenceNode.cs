using System.Collections.Generic;
using NPCs;
using UnityEngine;

namespace StateMachine.Nodes
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Sequence")]
    public class SequenceNode : Node
    {
        [SerializeField] List<Node> children;
        private int currentIndex = 0;

        public override NodeStatus Execute(NPC npc)
        {
            while (currentIndex < children.Count)
            {
                NodeStatus status = children[currentIndex].Execute(npc);
        
                if (status == NodeStatus.Running) return NodeStatus.Running;
                if (status == NodeStatus.Failure)
                {
                    currentIndex = 0;
                    return NodeStatus.Failure;
                }
        
                currentIndex++;
                if  (currentIndex >= children.Count) currentIndex = 0;
            }
    
            currentIndex = 0;
            return NodeStatus.Success;
        }
    }
}