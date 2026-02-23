
using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "Node", menuName = "State Machine/Node")]
    public abstract class Node : ScriptableObject
    {
        public abstract NodeStatus Execute(NPC npc);
    }
}