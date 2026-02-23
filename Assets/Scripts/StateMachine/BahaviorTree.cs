
using Unity.Behavior;
using UnityEngine;

namespace StateMachine
{
    public class BehaviorTree
    {
        private static BehaviorGraph behaviorGraph;
        private string name = "";
        public string description = "";

        public Node PrimaryNode;
        public Node[] Nodes;

        private void Start()
        {
            behaviorGraph = ScriptableObject.CreateInstance<BehaviorGraph>();
            behaviorGraph.name = name;
        }
    }
}
