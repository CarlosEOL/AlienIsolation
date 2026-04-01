using System.Collections.Generic;
using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Nodes/Parallel")]
    public class ParallelNode : Node
    {
        public List<Node> children = new();

        public override NodeStatus Execute(NPC npc) 
        {
            int runningCount = 0;

            foreach (Node child in children)
            {
                NodeStatus status = child.Execute(npc);
                if (status == NodeStatus.Failure) return NodeStatus.Failure;
                if (status == NodeStatus.Running) runningCount++;
            }

            return runningCount > 0 ? NodeStatus.Running : NodeStatus.Success;
        }
    }
}