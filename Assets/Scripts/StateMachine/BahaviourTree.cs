
using NPCs;
using Unity.Behavior;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Behaviour Tree", menuName = "State Machine/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        private static BehaviorGraph behaviorGraph;
        public string name;
        public string description;

        public Node PrimaryNode;

        public bool CanRepeat = true;

        public BehaviourTree(string Name, Node[] Nodes, NPC Npc)
        {
            name = Name;
            behaviorGraph = CreateInstance<BehaviorGraph>();
            behaviorGraph.name = Name;
        }
        
        public void Tick(NPC npc)
        {
            if (PrimaryNode != null) 
            {
                NodeStatus status = PrimaryNode.Execute(npc);
                
                if (CanRepeat && status != NodeStatus.Running)
                {
                    // Reset and rerun from top next tick
                    PrimaryNode.Reset(); 
                }
            }
        }
    }
}
