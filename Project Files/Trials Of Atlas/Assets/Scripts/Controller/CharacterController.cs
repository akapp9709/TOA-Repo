using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private PlayerControls _controls;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerSO playerStats;
    [SerializeField] private Transform cameraTransform;

    public float currentStamina, maxStamina;

    private bool _isMoving, _isSprinting, _canSprint;
    private float _sprintVal;
    private Rigidbody _rb;

    public Vector2 _inputVec;
    public Vector3 moveDirection;
    // Start is called before the first frame update
    private void Start()
    {
        SetupInputs();
        _rb = GetComponent<Rigidbody>();

        maxStamina = playerStats.maxStamina;
    }

    private void SetupInputs()
    {
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
        float currentSpeed = playerStats.runSpeed;
        if (_isSprinting)
        {
            _sprintVal = Mathf.Lerp(_sprintVal, 1, 0.01f);
            currentStamina -= Time.deltaTime * playerStats.staminaDepletion;
            currentSpeed = playerStats.sprintSpeed;
        }
        else
        {
            _sprintVal = Mathf.Lerp(_sprintVal, 0, 0.01f);
            currentStamina += Time.deltaTime * playerStats.staminaRegen;
        }
        anim.SetFloat("Sprint", _sprintVal);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina == 0)
        {
            _isSprinting = false;
        }

        var directionDesired = new Vector3(_inputVec.x, 0f, _inputVec.y);
        directionDesired = directionDesired.x * CamRt + directionDesired.z * CamFwd;
        directionDesired.y = 0f;

        moveDirection = Vector3.Lerp(moveDirection, directionDesired, 0.09f);
        if (_isMoving)
        {
            moveDirection = moveDirection.normalized;
        }

        _rb.velocity = moveDirection * currentSpeed;
        transform.LookAt(transform.position + moveDirection);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _isMoving = true;
                break;
            case InputActionPhase.Performed:
                _inputVec = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                _inputVec = Vector2.zero;
                _isMoving = false;
                break;
        }
        anim.SetBool("isMoving", _isMoving);
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        _isSprinting = !_isSprinting;
    }

    private Vector3 CamFwd => cameraTransform.forward;
    private Vector3 CamRt => cameraTransform.right;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + moveDirection * 2);
    }
}
