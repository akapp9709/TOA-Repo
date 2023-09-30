using System.Collections;
using UnityEngine;

namespace AJK
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform _cameraTransform;
        private InputHandler _input;
        private Vector3 _moveDirection;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimationHandler animHandler;

        public Rigidbody _rb;

        public GameObject normalCamera;

        [SerializeField] private PlayerSO playerStats;
        private float _rotationSpeed = 10;

        public bool isDodging = false;
        public Vector3 dodgeVelocity;
        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<InputHandler>();
            animHandler = GetComponentInChildren<AnimationHandler>();
            _cameraTransform = normalCamera.transform;
            myTransform = transform;
            animHandler.Initialize();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            _input.TickInput(delta);
            HandleMovement(delta);
            animHandler.SetAnimationValuesBool("isDodging", isDodging);

        }

        private void HandleMovement(float delta)
        {
            if (_input.isInteracting)
                return;

            _moveDirection = _cameraTransform.forward * _input.vertical;
            _moveDirection += _cameraTransform.right * _input.horizontal;
            _moveDirection.y = 0;
            _moveDirection.Normalize();

            float speed;

            if (Mathf.Abs(_input.moveAmount) < 0.55f)
            {
                speed = playerStats.walkSpeed;
                _input.isSprinting = false;
            }
            else if (Mathf.Abs(_input.moveAmount) > 0.55f && !_input.isSprinting)
            {
                speed = playerStats.runSpeed;
            }
            else
                speed = playerStats.sprintSpeed;

            _moveDirection *= speed;
            var dV = DodgeV(speed, dodgeVelocity, Time.time);

            var projectedV = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            _rb.velocity = projectedV + dV;

            animHandler.UpdateAnimatorValues(_input.moveAmount, 0);

            if (animHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region Movement

        private Vector3 _normalVector;
        private Vector3 _targetPosition;

        private void HandleRotation(float delta)
        {
            var targetDirection = Vector3.zero;
            var moveOverride = _input.moveAmount;


            targetDirection = _cameraTransform.forward * _input.vertical;
            targetDirection += _cameraTransform.right * _input.horizontal;

            targetDirection.Normalize();
            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
            {
                targetDirection = myTransform.forward;
            }

            var rs = _rotationSpeed;
            var tr = Quaternion.LookRotation(targetDirection);
            var targetRot = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRot;
        }

        public void HandleDodge()
        {
            if (_input.moveAmount == 0)
                return;

            var targetDirection = _cameraTransform.forward * _input.vertical;
            targetDirection += _cameraTransform.right * _input.horizontal;

            dodgeVelocity = _input.mouseY > 0 ? transform.forward * -playerStats.dodgeSpeed : _moveDirection;

            var targetRot = Quaternion.LookRotation(targetDirection);
            myTransform.rotation = targetRot;

            animHandler.TriggerTargetAnimation("Dodge", false);
            animHandler.StopRotation();
            StartCoroutine(DodgeRoutine());
        }

        private Vector3 DodgeV(float currentSpeed, Vector3 dodgeVelocity, float dodgeStartTime)
        {
            var dodgeV = isDodging
                ? dodgeVelocity.normalized *
                  EaseFunctions.AccelerateValue(dodgeStartTime,
                      playerStats.dodgeTime,
                      Time.time,
                      currentSpeed,
                      playerStats.dodgeSpeed,
                      EaseFunctions.EaseType.CircEaseOut)
                : Vector3.zero;
            return dodgeV;
        }

        private IEnumerator DodgeRoutine()
        {
            isDodging = true;
            yield return new WaitForSecondsRealtime(0.3f);
            isDodging = false;
            animHandler.CanRotate();
        }

        #endregion
    }
}

