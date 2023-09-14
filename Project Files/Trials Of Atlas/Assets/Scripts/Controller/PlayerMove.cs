using System;
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
    private Animator _anim;
    
    public Vector3 _inputDirection;

    private bool _isSprinting = false;
    private float _currentSprintVal = 0;
    private bool _isDodging = false;
    private Vector3 _dodgeVelocity;
    private float _dodgeStartTime;

    private void OnDrawGizmos()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // _anim = GetComponent<Animator>();
        
        _controls = new PlayerControls();
        _controls.Main.Enable();
        
        _controls.Main.Move.started += OnMove;
        _controls.Main.Move.performed += OnMove;
        _controls.Main.Move.canceled += OnMove;

        // _controls.Main.Sprint.started += ToggleSprint;
        // //_controls.Main.Sprint.canceled += ToggleSprint;
        //
        // _controls.Main.Dodge.started += OnDodge;
    }

    // Update is called once per frame
    void Update()
    {
        var sprintVal = _isSprinting ? 1 : 0;
        _currentSprintVal = Mathf.Lerp(_currentSprintVal, sprintVal, 0.01f);
        // _anim.SetFloat("Sprint", _currentSprintVal);
        
        var camFwd = CamFwd;
        var camRt = CamRt;

        var currentSpeed = _isSprinting ? playerStats.sprintSpeed : playerStats.runSpeed;

        var desiredDirection = _inputDirection.z * camFwd + _inputDirection.x * camRt;
        desiredDirection.y = 0f;

        var dodgeV = DodgeV(currentSpeed);

        _rb.velocity = desiredDirection.normalized * currentSpeed + dodgeV;

        transform.LookAt(transform.position + desiredDirection);
    }

    private Vector3 DodgeV(float currentSpeed)
    {
        var dodgeV = _isDodging
            ? _dodgeVelocity.normalized *
              EaseFunctions.AccelerateValue(_dodgeStartTime,
                  playerStats.dodgeTime,
                  Time.time,
                  currentSpeed,
                  playerStats.dodgeSpeed,
                  EaseFunctions.EaseType.CircEaseOut)
            : Vector3.zero;
        return dodgeV;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        var inVec = context.ReadValue<Vector2>();
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // _anim.SetBool("isMoving", true);
                break;
            case InputActionPhase.Performed:
                _inputDirection = new Vector3(inVec.x, 0, inVec.y);
                break;
            case InputActionPhase.Canceled:
                _inputDirection = Vector3.zero;
                // _anim.SetBool("isMoving", false);
                _isSprinting = false;
                break;
        }
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        _isSprinting = !_isSprinting;
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        if (_isDodging)
            return;

        var dVec = _inputDirection;
        if (dVec == Vector3.zero)
        {
            dVec = CamFwd * -1;
        }

        _dodgeVelocity = dVec.z * CamFwd + dVec.x * CamRt;
        _dodgeStartTime = Time.time;

        StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        _isDodging = true;
        yield return new WaitForSeconds(playerStats.dodgeTime);
        _isDodging = false;
    }

    private Vector3 CamFwd
    {
        get
        {
            var vec = cameraTransform.forward;
            vec.y = 0;
            return vec;
        }
    }

    private Vector3 CamRt
    {
        get
        {
            var vec = cameraTransform.right;
            vec.y = 0;
            return vec;
        }
    }
}
