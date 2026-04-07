
using System;
using System.Collections.Generic;
using Interactable;
using StateMachine;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Basic;
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
        [SerializeField] BehaviourTree protectBevaBehaviourTree;
        
        [SerializeField] private float separationRadius = 2f;
        [SerializeField] private float separationStrength = 1.5f;

        private Transform _currentTarget;
        public Vector3 targetLinearVelocity;
        public Transform Target
        {
            get => _currentTarget;
            set
            {
                _currentTarget = value;
                Debug.Log($"Target Changed to {_currentTarget.name} for NPC {name}");
                OnTargetChanged?.Invoke(_currentTarget);
                
                SetAgentToDefault();

                if (_currentTarget.TryGetComponent(out Controller _))
                {
                    if (!isEnemy)
                    {
                        targetLinearVelocity = Controller._linearVelocity;
                        SetAgentToFlocking();
                    }
                }


                if (_currentTarget.TryGetComponent(out NavMeshAgent a))
                {
                    if (!isEnemy)
                    {
                        targetLinearVelocity = a.velocity;
                        SetAgentToFlocking();
                    }
                }
            }
        }

        [SerializeField] public List<Transform> pointsOfInterests = new();
        private List<Vector3> _pointsOfInterests = new();

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if  (_isRunning != value)
                {
                    _isRunning = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }

        bool _hasLineOfSight;

        public bool HasSetPOI;
        
        public Action<bool> OnValueChanged;
        public Action<Transform> OnTargetChanged;
        
        [Header("Flocking Related Settings")]
        [SerializeField] public int MaxRecruitmentAmt = 3;
        [SerializeField] public List<NPC> EnlistedNPC = new();
        private int _currentRecruitmentAmt = 0;
        
        private Collider[] _cachedNeighbors = new Collider[0];
        private float _separationUpdateTimer = 0f;
        [SerializeField] private float separationUpdateInterval = 0.2f;
        
        private void Awake()
        {
            //Checksum for value change: if isRunning, speed change to run. if Target is a player or a distraction, it isRunning.
            OnTargetChanged = currentTarget => { IsRunning = currentTarget.gameObject.CompareTag("Player"); };
            OnValueChanged = isItRunning => { agent.speed = isItRunning ? runSpeed : walkSpeed; };
            
            agent.enabled = true;
            
            defaultBehaviorTree = Instantiate(defaultBehaviorTree);
            protectBevaBehaviourTree = Instantiate(protectBevaBehaviourTree);
            
            Target = new GameObject().transform;
            Target.position = transform.position;
            
            HasSetPOI = CheckPOI();
            Debug.Log(HasSetPOI ? "Actor has POIs." : $"Generating POI for {name}");
        }
        
        void Update() 
        {
            if (currentGoals == IStateAndGoals.NPCGoals.Idle) return;
            if (currentGoals == IStateAndGoals.NPCGoals.Protect && canMove)
            {
                protectBevaBehaviourTree.Tick(this);
                return;
            }
            
            if (defaultBehaviorTree != null && canMove) 
            {
                defaultBehaviorTree.Tick(this);
            }
        }

        public bool CheckPOI()
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
                Target.name = "Target";
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

        public bool HasTargetInSight()
        {
            return IsTargetInCone(transform, 15f, 60f);
        }

        public bool CheckIsInTargetRange()
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

        public Vector3 CalculateSeparationOffset()
        {
            _separationUpdateTimer -= Time.deltaTime;
            if (_separationUpdateTimer <= 0f)
            {
                _cachedNeighbors = Physics.OverlapSphere(transform.position, separationRadius);
                _separationUpdateTimer = separationUpdateInterval;
            }

            Vector3 separationOffset = Vector3.zero;
            foreach (Collider neighbor in _cachedNeighbors)
            {
                if (neighbor.gameObject == gameObject) continue;
                if (!neighbor.TryGetComponent<NPC>(out _)) continue;

                Vector3 awayFromNeighbor = transform.position - neighbor.transform.position;
                float distance = awayFromNeighbor.magnitude;
                if (distance > 0)
                    separationOffset += awayFromNeighbor.normalized / distance;
            }

            return separationOffset * separationStrength;
        }
        
        private bool IsTargetInCone(Transform npc, float maxDist, float maxAngle)
        {
            int rayCount = 5;
            for (int i = 0; i < rayCount; i++)
            {
                float t = rayCount == 1 ? 0.5f : (float)i / (rayCount - 1);
                float angle = Mathf.Lerp(-maxAngle / 2, maxAngle / 2, t);
                Vector3 rayDir = Quaternion.AngleAxis(angle, npc.up) * npc.forward;

                if (Physics.Raycast(npc.position, rayDir, out RaycastHit hit, maxDist))
                {
                    if (hit.collider.gameObject.layer == 8) continue;

                    if (!isEnemy && CheckTargetTag(hit.collider.gameObject)) continue;
                    if (isEnemy && !CheckTargetTag(hit.collider.gameObject)) continue;

                    Target = hit.transform;
                    currentState = IStateAndGoals.NPCState.Pursue;
                    return true;
                }
            }

            return false;
        }

        public virtual void Attack()
        {
            Debug.Log("Doing Attack Logic.");
        }

        // if target is player or friendly return true
        public bool CheckTargetTag(GameObject targetTag)
        {
            if (targetTag.CompareTag("Player") || targetTag.CompareTag("Friendly")) return true;
            return false;
        }

        void SetAgentToFlocking()
        {
            agent.autoBraking = false;
            agent.stoppingDistance = 0.1f;
            agent.angularSpeed = 70f;
            agent.acceleration = 18f;
        }
        
        void SetAgentToDefault()
        {
            agent.autoBraking = true;
            agent.stoppingDistance = 0.5f;
            agent.angularSpeed = 100f;
            agent.acceleration = 10f;
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

