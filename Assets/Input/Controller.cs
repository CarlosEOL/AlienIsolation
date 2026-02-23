using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        _inputs = new PlayerInputs();
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
            if (canJump)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                canJump = false;
            }
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

    private void Update()
    {
        Debug.Log($"X: {x}, Y: {y}");
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
        _yRotation += mousex * sensitivity;
        _xRotation += -(mousey * sensitivity);

        // Apply only the accumulated rotation
        Quaternion targetRotation = Quaternion.Euler(0, _yRotation, 0);
        rb.MoveRotation(targetRotation); 
        
        Quaternion camRot = Quaternion.Euler(_xRotation, 0, 0);
        cam.transform.rotation = camRot;
        
        Vector3 forward = cam.transform.forward.normalized;
        Vector3 right = cam.transform.right.normalized;
        
        // Calculate move direction based on inputs (assuming moveInputX/Y are move axes)
        Vector3 moveDir = (forward * y) + (right * x);
        float currentSpeed = _isRunning ? speed * 2f : speed;
        
        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }
}

