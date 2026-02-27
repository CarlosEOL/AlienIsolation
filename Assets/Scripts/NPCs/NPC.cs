
using System;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPCs
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] public float walkSpeed;
        [SerializeField] public float runSpeed;
        
        [SerializeField] bool canMove = true;
        
        [SerializeField] BehaviourTree behaviorTree;
        [SerializeField] ScriptableObject currentActionSO;

        private Transform _currentTarget;
        public Transform Target
        {
            get => _currentTarget;
            set
            {
                _currentTarget = value;
                OnTargetChanged?.Invoke(_currentTarget);
            }
        }

        [SerializeField] public List<Transform> pointsOfInterests = new List<Transform>();
        private List<Vector3> _pointsOfInterests = new List<Vector3>();

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if  (_isRunning != value)
                {
                    _isRunning = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        bool _hasLineOfSight;

        public bool HasSetPOI
        {
            get;
            private set;
        }
        
        public Action<bool> OnValueChanged;
        public Action<Transform> OnTargetChanged;
        
        private void Awake()
        {
            agent.enabled = true;
            
            behaviorTree = Instantiate(behaviorTree);
            
            Target = transform;

            Debug.Log(CheckPOI() ? "Actor has POIs." : "Generating POI for Actor");

            ChangeTarget();
            //Checksum for value change: if isRunning, speed change to run. if Target is a player or a distraction, it isRunning.
            OnValueChanged = isItRunning => { agent.speed = isItRunning ? runSpeed : walkSpeed; };
            OnTargetChanged = currentTarget => { IsRunning = currentTarget.gameObject.CompareTag("Player"); };
        }
        
        void Update() 
        {
            if (behaviorTree != null && canMove) 
            {
                behaviorTree.Tick(this);
            }
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, Target.position) < 5f)
            {
                ChangeTarget();
            }
        }

        bool CheckPOI()
        {
            if (pointsOfInterests.Count > 0)
            {
                // Convert to Vec3
                foreach (var p in pointsOfInterests)
                {
                    _pointsOfInterests.Add(p.position);
                    Debug.Log($"x: {p.position.x}, y: {p.position.y}, z: {p.position.z}");
                }
                return true;
            }
            
            // Add Random Point to POI in case if there aren't any targets.
            for (int i = 0; i < 3; i++)
            {
                _pointsOfInterests.Add(new Vector3(transform.position.x + Random.Range(-50, 50), 0, transform.position.z + Random.Range(-50, 50)));
            }
            return false;
        }

        void ChangeTarget()
        {
            int index = 0;
            if (_pointsOfInterests.Count > 0)
                index = Random.Range(0, _pointsOfInterests.Count - 1);
            else
                CheckPOI();    
            
            Target.position = _pointsOfInterests[index];
            _pointsOfInterests.RemoveAt(index);
        }
    }
}

