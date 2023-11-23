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
        private PlayerInputCLass _controls;
        public Rigidbody _rb;
        private float _currentSpeed;
        private float _moveAmount;
        public bool isGrounded, isDodging;

        private Animator _anim;
        public bool isSprinting;
        public float findNormalRadius;
        public Vector3 checkOffset = new Vector3(0f, 1f, 0f);
        public LayerMask groundLayers;
        private Vector3 normalCheck1, normalCheck2, groundHeight1, groundHeight2, groundNorm, transformedGo, _dodgeDir;

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(normalCheck1, 0.2f);
            Gizmos.DrawSphere(normalCheck2, 0.2f);
            Gizmos.DrawSphere(groundHeight1, 0.1f);
            Gizmos.DrawSphere(groundHeight2, 0.1f);

            Gizmos.DrawLine(transform.position, transform.position + groundNorm.normalized);
            Gizmos.DrawLine(transform.position, transform.position + transformedGo.normalized);
        }

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
            _controls = GetComponent<PlayerManager>().inputHandler;
            _controls.EnableMainInput();
            _controls.SprintToggle = ToggleSprint;
            _controls.DodgeAction = Dodge;
        }

        private void Update()
        {
            inVec = _controls.inputVector;
            _moveAmount = _controls.moveAmount;

            PlaceNormalChecks();

            Vector3 gravV = Vector3.zero;
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, groundLayers);
            if (!isGrounded)
            {
                gravV = Vector3.down * 6f;
            }


            if (Mathf.Abs(_moveAmount) < 0.15f)
            {
                _moveAmount = 0;
                isSprinting = false;
            }
            else if (Mathf.Abs(_moveAmount) < 0.55f)
            {
                _moveAmount = 0.5f;
                isSprinting = false;
            }
            else if (Mathf.Abs(_moveAmount) >= 0.55f)
            {
                _moveAmount = 1;
            }

            if (isSprinting && Mathf.Abs(_moveAmount) >= 0.55f)
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
            _currentSpeed = Mathf.Lerp(_currentSpeed, speed, 0.5f);

            var dodgeV = Vector3.zero;
            if (isDodging)
            {
                dodgeV = _dodgeDir * playerStats.dodgeSpeed;
            }



            var desiredDirection = inVec.x * cameraTransform.right + inVec.z * cameraTransform.forward;
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var desiredV = desiredDirection * _currentSpeed;
            var goV = Vector3.Lerp(_rb.velocity, desiredV, 0.5f);


            if (Vector3.Dot(goV, desiredV) > 0.9)
            {
                goV = desiredV;
            }
            transformedGo = Vector3.ProjectOnPlane(goV, groundNorm);

            _rb.velocity = transformedGo + gravV + dodgeV;

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

        private void PlaceNormalChecks()
        {
            normalCheck1 = transform.position + transform.forward * findNormalRadius + checkOffset;
            normalCheck2 = transform.position + transform.forward * -findNormalRadius + checkOffset;

            var hit1 = new RaycastHit();
            var hit2 = new RaycastHit();

            var groundLevel1 = Physics.Raycast(normalCheck1, Vector3.down, out hit1, 2f);
            var groundLevel2 = Physics.Raycast(normalCheck2, Vector3.down, out hit2, 2f);

            groundHeight1 = hit1.point;
            groundHeight2 = hit2.point;

            var groundSlope = groundHeight2 - groundHeight1;
            groundNorm = Vector3.Cross(transform.right, groundSlope);
        }

        private void ToggleSprint()
        {
            isSprinting = !isSprinting;
        }

        private void Dodge()
        {
            if (!GetComponent<PlayerManager>().canUseStamina)
            {
                return;
            }

            StartCoroutine(DodgeRoutine());
            _dodgeDir = inVec.x * cameraTransform.right + inVec.z * cameraTransform.forward;
            _anim.SetTrigger("Dodge");
        }

        private IEnumerator DodgeRoutine()
        {
            isDodging = true;
            yield return new WaitForSeconds(playerStats.dodgeTime);
            isDodging = false;
        }
    }
}

