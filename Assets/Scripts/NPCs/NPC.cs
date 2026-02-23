
using StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] float acceleration;
        
        [SerializeField] bool canMove = true;
        
        [SerializeField] BehaviourTree behaviorTree;
        [SerializeField] ScriptableObject currentActionSO;
        
        public Transform target;
        
        private void Awake()
        {
            agent.enabled = true;
            agent.acceleration = acceleration;
        }
        
        void Update() 
        {
            if (behaviorTree != null) 
            {
                behaviorTree.Tick(this);
            }
        }
    }
}

