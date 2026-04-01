
using System;
using System.Collections.Generic;
using Interactable;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPCs
{
    public class NPC : MonoBehaviour, IStateAndGoals, IInteractables
    {
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] public float walkSpeed;
        [SerializeField] public float runSpeed;
        
        [SerializeField] public bool canMove = true;
        [SerializeField] public bool isEnemy = false;
        [SerializeField] public bool isLeader = false;
        [SerializeField] public bool canFlock = true;
        [SerializeField] public IStateAndGoals.NPCGoals currentGoals = IStateAndGoals.NPCGoals.Protect;
        [SerializeField] public IStateAndGoals.NPCState currentState = IStateAndGoals.NPCState.Idle;
        
        [SerializeField] BehaviourTree defaultBehaviorTree;
        [SerializeField] BehaviourTree flockBehaviors;
        
        [SerializeField] private float separationRadius = 2f;
        [SerializeField] private float separationStrength = 1.5f;

        private Transform _currentTarget;
        public Vector3 TargetLinearVelocity = Vector3.zero;
        public Transform Target
        {
            get => _currentTarget;
            set
            {
                _currentTarget = value;
                Debug.Log($"Target Changed to {_currentTarget.name} for NPC {name}");
                OnTargetChanged?.Invoke(_currentTarget);
                
                if (_currentTarget.TryGetComponent(out Controller _))
                    TargetLinearVelocity = Controller._linearVelocity;
                
                if (_currentTarget.TryGetComponent(out NavMeshAgent a))
                    TargetLinearVelocity = a.velocity;
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
        
        [Header("Flocking Related Settings")]
        [SerializeField] public int MaxRecruitmentAmt = 3;
        [SerializeField] public List<NPC> EnlistedNPC = new();
        private int _currentRecruitmentAmt = 0;
        
        private void Awake()
        {
            //Checksum for value change: if isRunning, speed change to run. if Target is a player or a distraction, it isRunning.
            OnTargetChanged = currentTarget => { IsRunning = currentTarget.gameObject.CompareTag("Player"); };
            OnValueChanged = isItRunning => { agent.speed = isItRunning ? runSpeed : walkSpeed; };
            
            agent.enabled = true;
            
            defaultBehaviorTree = Instantiate(defaultBehaviorTree);
            flockBehaviors = Instantiate(flockBehaviors);
            
            Target = transform;

            HasSetPOI = CheckPOI();
            Debug.Log(HasSetPOI ? "Actor has POIs." : "Generating POI for Actor");
        }
        
        void Update() 
        {
            if (currentGoals == IStateAndGoals.NPCGoals.Idle) return;
            if (currentGoals == IStateAndGoals.NPCGoals.Protect && canMove)
            {
                flockBehaviors.Tick(this);
                return;
            }
            
            if (defaultBehaviorTree != null && canMove) 
            {
                defaultBehaviorTree.Tick(this);
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

        public void ChangeTarget()
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

        protected virtual bool HasTargetInsight()
        {
            return IsTargetInCone(this.transform, Target, 10f, 65f);
        }

        public virtual bool CheckIsInTargetRange()
        {
            if (Vector3.Distance(transform.position, Target.position) < 0.8f)
            {
                return true;
            }
            
            return false;
        }

        public string GetName()
        {
            return name;
        }

        public void Interact(Controller controller)
        {
            Debug.Log($"Added {name} to Party!");
            currentGoals = IStateAndGoals.NPCGoals.Protect;
            currentState = IStateAndGoals.NPCState.Pursue;
        }

        public Vector3 CalculateSepereationOffset()
        {
            // Separation offset
            Vector3 separationOffset = Vector3.zero;
            Collider[] neighbors = Physics.OverlapSphere(transform.position, separationRadius);
            foreach (Collider neighbor in neighbors)
            {
                if (neighbor.gameObject == gameObject) continue;
                if (!neighbor.TryGetComponent<NPC>(out _)) continue;

                Vector3 awayFromNeighbor = transform.position - neighbor.transform.position;
                float distance = awayFromNeighbor.magnitude;
                if (distance > 0)
                    separationOffset += awayFromNeighbor.normalized / distance; // closer = stronger push
            }
            return separationOffset *= separationStrength;
        }
        
        public bool IsTargetInCone(Transform npc, Transform target, float maxDist, float maxAngle)
        {
            Vector3 dirToTarget = (target.position - npc.position).normalized;
            float distance = Vector3.Distance(npc.position, target.position);

            if (distance < maxDist)
            {
                // Check if target is within the FOV angle
                if (Vector3.Angle(npc.forward, dirToTarget) < maxAngle / 2)
                {
                    // Raycast to check for walls/obstacles
                    if (!Physics.Linecast(npc.position, target.position, layerMask: target.gameObject.layer))
                    {
                        Debug.Log("I SEE YOU!");
                        return true; // Detected!
                    }
                }
            }
            return false;
        }

        private void OnDestroy()
        {
            if (Target.TryGetComponent(out Controller controller))
            {
                controller.EnlistedNPC.Remove(this);
            }
            if (Target.TryGetComponent(out NPC npc))
            {
                npc.EnlistedNPC.Remove(this);
            }
        }
    }
}

