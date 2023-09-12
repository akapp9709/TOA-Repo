using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AJK
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        private PlayerControls _inputActions;
        private PlayerCamera _cameraHandler;

        private Vector2 _movementInput, _cameraInput;

        public void Awake()
        {
            _cameraHandler = PlayerCamera.Singleton;
            
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (_cameraHandler != null)
            {
                _cameraHandler.FollowTarget(delta);
                _cameraHandler.HandleRotation(delta, mouseX, mouseY);
            }
        }

        public void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                _inputActions.Main.Move.performed += context => _movementInput = context.ReadValue<Vector2>();
                _inputActions.Main.Look.performed += context => _cameraInput = context.ReadValue<Vector2>();
            }
            
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = _movementInput.x;
            vertical = _movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = _cameraInput.x;
            mouseY = _cameraInput.y;
        }
    }
}

