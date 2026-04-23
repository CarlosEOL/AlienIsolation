using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Attack")]
    public class AttackAction : Node
    {
        [SerializeField] private float attackCooldown = 1f;
        private float _cooldownTimer = 0f;
        
        public override NodeStatus Execute(NPC npc)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer > 0f) return NodeStatus.Running;
            Debug.Log($"Attack called by {npc.name} | Timer: {_cooldownTimer}");
            
            npc.Attack();
            _cooldownTimer = attackCooldown;
            return NodeStatus.Success;
        }
        
        public override void Reset()
        {
            _cooldownTimer = 0f;
        }
    }
}