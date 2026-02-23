using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "Scriptable Objects/Node")]
    public class Node : ScriptableObject
    {
        private TrueFalseDecision _evaluateDecision;
        
        public Node Parent;
        public Node[] Nodes;
        
        public class NodeFunctions : MonoBehaviour
        {
            
        }

    }
}