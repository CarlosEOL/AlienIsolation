
using NPCs;
using Unity.Behavior;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Behavior Tree", menuName = "Scriptable Objects/Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        private static BehaviorGraph behaviorGraph;
        public string name;
        public string description;

        public Node PrimaryNode;
        public Node[] Nodes;
        public TrueFalseDecision DefaultDecision;

        public NPC npc { get; private set; }

        public BehaviorTree(string Name, Node[] Nodes, NPC Npc)
        {
            name = Name;
        }

        private void Start()
        {
            behaviorGraph = ScriptableObject.CreateInstance<BehaviorGraph>();
            behaviorGraph.name = name;
        }
    }
}
