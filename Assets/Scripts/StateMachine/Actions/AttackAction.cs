using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Attack")]
    public class AttackAction : Node
    {
        public override NodeStatus Execute(NPC npc)
        {
            npc.Attack();
            return NodeStatus.Success;
        }
    }
}