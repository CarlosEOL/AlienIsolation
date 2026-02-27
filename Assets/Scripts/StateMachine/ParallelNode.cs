using System.Collections.Generic;
using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Parallel")]
    public class ParallelNode : Node
    {
        public List<Node> children = new List<Node>();

        public override NodeStatus Execute(NPC npc) 
        {
            bool anyChildRunning = false;
            int successCount = 0;

            foreach (var child in children) 
            {
                NodeStatus status = child.Execute(npc);

                if (status == NodeStatus.Failure) 
                    return NodeStatus.Failure; // Fail immediately if any child fails
            
                if (status == NodeStatus.Running) 
                    anyChildRunning = true;
            
                if (status == NodeStatus.Success) 
                    successCount++;
            }

            // If at least one child is still working, the whole Parallel node is 'Running'
            return anyChildRunning ? NodeStatus.Running : NodeStatus.Success;
        }
    }
}