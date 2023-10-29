using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AJK
{
    public class PlayerLocomotion : MonoBehaviour
    {
        public Transform cameraTransform;
        public PlayerSO playerStats;
        private float _walk, _run, _sprint;
        public Vector3 inVec;
        private PlayerControls _controls;
        public Rigidbody _rb;
        private float _currentSpeed;
        private float _moveAmount;

        private Animator _anim;
        private bool _isSprinting = false;

        private void Start()
        {
            InitializeControls();

            _rb = GetComponent<Rigidbody>();
            _walk = playerStats.walkSpeed;
            _run = playerStats.runSpeed;
            _sprint = playerStats.sprintSpeed;

            _anim = GetComponentInChildren<Animator>();

        }

        private void InitializeControls()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _controls.Main.Enable();
            _controls.Main.Move.performed += OnMoveStart;
            _controls.Main.Sprint.started += ToggleSprint;
        }

        private void Update()
        {
            if (Mathf.Abs(_moveAmount) < 0.15f)
            {
                _moveAmount = 0;
                _isSprinting = false;
            }
            else if (Mathf.Abs(_moveAmount) < 0.55f)
            {
                _moveAmount = 0.5f;
                _isSprinting = false;
            }
            else if (Mathf.Abs(_moveAmount) >= 0.55f)
            {
                _moveAmount = 1;
            }

            if (_isSprinting && Mathf.Abs(_moveAmount) >= 0.55f)
            {
                _moveAmount = 2;
            }

            var speed = 0f;
            switch (_moveAmount)
            {
                case 0:
                    speed = 0;
                    break;
                case 0.5f:
                    speed = _walk;
                    break;
                case 1:
                    speed = _run;
                    break;
                case 2:
                    speed = _sprint;
                    break;
            }
            _currentSpeed = Mathf.Lerp(_currentSpeed, speed, 0.1f);



            var desiredDirection = inVec.x * cameraTransform.right + inVec.z * cameraTransform.forward;
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var desiredV = desiredDirection * _currentSpeed;
            var goV = Vector3.Lerp(_rb.velocity, desiredV, 0.5f);

            if (Vector3.Dot(goV, desiredV) > 0.9)
            {
                goV = desiredV;
            }

            _rb.velocity = goV;

            _anim.SetFloat("Vertical", _moveAmount);

            if (_anim.GetBool("IsInteracting"))
                return;
            if (_moveAmount > 0)
            {
                var rot = transform.rotation;
                var targetRot = Quaternion.LookRotation(desiredDirection);

                rot = Quaternion.Slerp(rot, targetRot, 0.2f);
                transform.rotation = rot;
            }
        }

        private void OnMoveStart(InputAction.CallbackContext context)
        {
            var inv = context.ReadValue<Vector2>();
            inVec = new Vector3(inv.x, 0f, inv.y);
            _moveAmount = Mathf.Clamp01(Mathf.Abs(inv.x) + Mathf.Abs(inv.y));
        }

        private void ToggleSprint(InputAction.CallbackContext context)
        {
            _isSprinting = !_isSprinting;
        }
    }
}

