using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerSO playerStats;
    
    private PlayerControls _controls;
    private Rigidbody _rb;
    public Vector3 _inputDirection;

    private bool _isSprinting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _controls = new PlayerControls();
        _controls.Main.Enable();
        
        _controls.Main.Move.started += OnMove;
        _controls.Main.Move.performed += OnMove;
        _controls.Main.Move.canceled += OnMove;

        _controls.Main.Sprint.started += ToggleSprint;
        //_controls.Main.Sprint.canceled += ToggleSprint;
    }

    // Update is called once per frame
    void Update()
    {
        var camFwd = cameraTransform.forward;
        var camRt = cameraTransform.right;

        var currentSpeed = _isSprinting ? playerStats.sprintSpeed : playerStats.runSpeed;

        var desiredDirection = _inputDirection.z * camFwd + _inputDirection.x * camRt;
        desiredDirection.y = 0f;

        _rb.velocity = desiredDirection.normalized * currentSpeed;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        var inVec = context.ReadValue<Vector2>();
        switch (context.phase)
        {
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Performed:
                _inputDirection = new Vector3(inVec.x, 0, inVec.y);
                break;
            case InputActionPhase.Canceled:
                _inputDirection = Vector3.zero;
                break;
        }
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        _isSprinting = !_isSprinting;
    }
}
