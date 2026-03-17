
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

        [SerializeField] public List<Transform> pointsOfInterests = new();
        private List<Vector3> _pointsOfInterests = new();

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
            //Checksum for value change: if isRunning, speed change to run. if Target is a player or a distraction, it isRunning.
            OnTargetChanged = currentTarget => { IsRunning = currentTarget.gameObject.CompareTag("Player"); };
            OnValueChanged = isItRunning => { agent.speed = isItRunning ? runSpeed : walkSpeed; };
            
            agent.enabled = true;
            
            behaviorTree = Instantiate(behaviorTree);
            
            Target = transform;

            HasSetPOI = CheckPOI();
            Debug.Log(HasSetPOI ? "Actor has POIs." : "Generating POI for Actor");
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
            if (Vector3.Distance(transform.position, Target.position) < 0.5f)
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

                Target = pointsOfInterests[0];
                Debug.Log($"Current Target Location: {Target.position}");
                return true;
            }

            if (_pointsOfInterests.Count < 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    _pointsOfInterests.Add(new Vector3(transform.position.x + Random.Range(-50, 50), 0, transform.position.z + Random.Range(-50, 50)));
                }
                Target = new GameObject().transform;
                Target.name = "Point Of Interests";
                Target.position = _pointsOfInterests[0];
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

            Debug.Log($"Current Target Location: {Target.position}");
            
            Target.position = _pointsOfInterests[index];
            _pointsOfInterests.RemoveAt(index);
        }

        public virtual void HasPlayerInsight()
        {
            
        }
    }
}

