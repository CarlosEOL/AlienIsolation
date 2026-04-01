
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactable;
using NPCs;
using StateMachine;

[RequireComponent(typeof(PlayerInputs))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Controller : MonoBehaviour
{
    PlayerInputs _inputs;
    [SerializeField] new Camera camera;
    [SerializeField] Rigidbody rb;
    [SerializeField] TogglePerspective togglePerspective;
    [SerializeField] Transform cameraTarget;
    [SerializeField] TMP_Text txt;
    
    [Header("Player Settings")]
    [SerializeField]bool canJump = true;
    [SerializeField]private float speed;
    [SerializeField]private float turnSpeed;
    [SerializeField]private float jumpPower;
    [SerializeField]private float sensitivity = 0.5f;
    
    float x;
    float y;
    float mx;
    float my;
    private float _yRotation;
    private float _xRotation;

    private bool _isRunning;
    public static Vector3 _linearVelocity = Vector3.zero;
    
    [Header("Interactable Settings")]
    LayerMask _layerMask;
    private RaycastHit _hit;
    Interactables _currentInteractables;
    private string _interactableName;

    public bool isHiding;
    
    [SerializeField] public AudioSource audioClip;

    [Header("NPC Recruitment Settings")] 
    [SerializeField] private int MaxRecruitmentAmount = 5;
    [SerializeField] public List<NPC> EnlistedNPC = new();

    [Header("Gameplay Settings")] 
    [SerializeField] public float MaxHealth = 100f;
    private float _currentHealth;
    
    public static Controller Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _inputs = new PlayerInputs();
        _layerMask = LayerMask.GetMask("Interactable");
        _currentHealth = MaxHealth;
    }

    private void OnEnable()
    {
        _inputs.Enable();

        _inputs.Movement.Move.performed += HandleMove;
        _inputs.Movement.Move.canceled += HandleMove;
        
        _inputs.Movement.LookAround.performed += HandleCamRot;
        _inputs.Movement.LookAround.canceled += HandleCamRot;

        _inputs.Movement.Run.started += _ => _isRunning = true;
        _inputs.Movement.Run.canceled += _ => _isRunning = false;

        _inputs.Movement.Jump.performed += _ =>
        {
            if (canJump && !isHiding)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                canJump = false;
            }
        };

        _inputs.Movement.Interact.started += _ =>
        {
            if (_currentInteractables != null)
            {
                _currentInteractables.Interact(this);
                _currentInteractables = null;
            }
            else if (_currentInteractables == null)
            {
                if (_hit.collider.CompareTag("Interactable"))
                {
                    _currentInteractables = _hit.transform.GetComponent<Interactables>();
                    _currentInteractables.Interact(this);
                }
            }
            
            rb.GetComponent<Collider>().isTrigger = isHiding;
            rb.useGravity = !isHiding;
        };

        // UI Controls
        _inputs.UI.TogglePerspective.started += _ =>
        {
            togglePerspective.Toggle();
        };
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
    
    private void FixedUpdate()
    {
        // Handles in-world movement
        if (TogglePerspective.index == 0)
            MoveBaseOnCam(camera, y, x);
        else
            MoveBaseOnCam(cameraTarget, mx, my);
    }

    void HandleMove(InputAction.CallbackContext ctx)
    {
        var move = ctx.ReadValue<Vector2>();
        x = move.x;
        y = move.y;
    }
    
    void HandleCamRot(InputAction.CallbackContext ctx)
    {
        var move = ctx.ReadValue<Vector2>();
        mx = move.x;
        my = move.y;
    }

    private void MoveBaseOnCam(Camera cam, float forward, float right)
    {
        if (isHiding) return;
        
        HandleRaycasts(cam.transform, 10f);
        
        Vector3 target = (cam.transform.forward * forward) + (cam.transform.right * right);
        target = target.normalized;
        
        transform.position += new Vector3(target.x, 0f, target.z) * (_isRunning ? speed * 2f: speed) * Time.deltaTime;
        
        if (target != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(target);
            targetRot.eulerAngles = new Vector3(0, targetRot.eulerAngles.y, 0);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    private void MoveBaseOnCam(Transform cam, float mousex, float mousey)
    {
        if (isHiding) return;
        
        HandleRaycasts(cam, 10f);
        
        _yRotation += mousex * sensitivity;
        _xRotation += -(mousey * sensitivity);

        // Apply only the accumulated rotation
        Quaternion targetRotation = Quaternion.Euler(0, _yRotation, 0);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
        
        Quaternion camRot = Quaternion.Euler(_xRotation, _yRotation, 0);
        cam.transform.rotation = camRot;
        
        Vector3 forward = cam.transform.forward.normalized;
        Vector3 right = cam.transform.right.normalized;
        
        // Calculate move direction based on inputs (assuming moveInputX/Y are move axes)
        Vector3 moveDir = (forward * y) + (right * x);
        float currentSpeed = _isRunning ? speed * 2f : speed;
        _linearVelocity = moveDir * currentSpeed * Time.deltaTime;
        
        rb.MovePosition(rb.position + _linearVelocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }

    void HandleRaycasts(Transform origin, float maxDistance)
    {
        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, maxDistance, _layerMask))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                _hit = hit;
                txt.enabled = true;
                _interactableName = hit.transform.GetComponent<Interactables>().GetName();
                txt.text = $"Press E to interact with {_interactableName}";
            }
        }
        else
        {
            _hit = new RaycastHit();
            txt.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        if (other.TryGetComponent(out NPC npc) && EnlistedNPC.Count < MaxRecruitmentAmount)
        {
            Debug.Log("NPC in Range.");
            if (!npc.canFlock) return;

            Debug.Log("Adding NPC to Rank.");
            npc.Target = transform;
            npc.Interact(this);
            EnlistedNPC.Add(npc);
        }
    }

    public void AddHealth(float amount)
    {
        if (_currentHealth + amount > MaxHealth) _currentHealth = MaxHealth;
        else _currentHealth += amount;
    }
}