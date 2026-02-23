
using Unity.Behavior;
using UnityEngine;

namespace StateMachine
{
    public class BehaviorTree
    {
        private static BehaviorGraph behaviorGraph;
        private string _name = "";
        public string _description = "";

        public Node PrimaryNode;
        public Node[] Nodes;

        public NPC npc { get; private set; }

        public BehaviorTree(string name, Node[] nodes, NPC npc)
        {
            _name = name;
        }

        private void Start()
        {
            behaviorGraph = ScriptableObject.CreateInstance<BehaviorGraph>();
            behaviorGraph.name = _name;
        }
    }
}
