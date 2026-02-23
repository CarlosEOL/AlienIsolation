using System.Collections.Generic;
using NPCs;

namespace StateMachine
{
    public class Selector : Node
    {
        public List<Node> children = new List<Node>();

        public override NodeStatus Execute(NPC npc) 
        {
            foreach (var node in children) 
            {
                NodeStatus status = node.Execute(npc);
            
                // If any child is working or finished, the Selector is happy.
                if (status != NodeStatus.Failure) return status;
            }
            return NodeStatus.Failure; // Everyone failed.
        }
    }
}