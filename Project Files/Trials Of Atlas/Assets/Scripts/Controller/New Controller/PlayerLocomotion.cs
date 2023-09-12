using System;
using System.Collections;
using System.Collections.Generic;
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

        public new Rigidbody _rb;

        public GameObject normalCamera;

        [SerializeField] private PlayerSO playerStats;
        private float _rotationSpeed = 10;
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
            _moveDirection = _cameraTransform.forward * _input.vertical;
            _moveDirection += _cameraTransform.right * _input.horizontal;
            _moveDirection.y = 0;
            _moveDirection.Normalize();

            var speed = playerStats.runSpeed;
            _moveDirection *= speed;

            var projectedV = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            _rb.velocity = projectedV;
            
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
            var targetRot = Quaternion.Slerp(myTransform.rotation, tr, rs*delta);

            myTransform.rotation = targetRot;
        }

        #endregion
    }
}

