
using StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs
{
    public interface INpcActions
    {
        bool CanMove();
        void Wonder();
    }
    
    public class NPC : MonoBehaviour, INpcActions
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float acceleration;
        
        [SerializeField] bool canMove = true;
        
        private void Awake()
        {
            agent.enabled = true;
            agent.acceleration = acceleration;
        }

        bool INpcActions.CanMove()
        {
            return canMove;
        }

        public void Wonder()
        {
            if (canMove)
            {
                
            }
        }
    }
}

