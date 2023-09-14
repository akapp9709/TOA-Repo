using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AJK
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool bInput, dodgeFlag, isInteracting;

        private PlayerControls _inputActions;
        private PlayerCamera _cameraHandler;

        private Vector2 _movementInput, _cameraInput;

        public bool isSprinting = false;

        public delegate void Dodge();
        public Dodge OnDodgeEvent;

        public void Start()
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
            else
            {
                _cameraHandler = PlayerCamera.Singleton;
            }
        }

        public void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                _inputActions.Main.Move.performed += context => _movementInput = context.ReadValue<Vector2>();
                _inputActions.Main.Look.performed += context => _cameraInput = context.ReadValue<Vector2>();
                _inputActions.PlayerActions.Dodge.started += OnDodge;
                _inputActions.PlayerActions.Sprint.started += ToggleSprint;
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
            DodgeInput(delta);
        }

        private void MoveInput(float delta)
        {
            if (!isInteracting)
            {
                horizontal = _movementInput.x;
                vertical = _movementInput.y;
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                mouseX = _cameraInput.x;
                mouseY = _cameraInput.y;
            }
        }

        private void DodgeInput(float delta)
        {
            if (bInput)
            {
                dodgeFlag = true;
                bInput = false;
            }
            else
                dodgeFlag = false;
        }

        private void OnDodge(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    bInput = true;
                    break;
                case InputActionPhase.Canceled:
                    bInput = false;
                    break;
            }
        }

        private void ToggleSprint(InputAction.CallbackContext context)
        {
            isSprinting = !isSprinting;
        }
    }
}

