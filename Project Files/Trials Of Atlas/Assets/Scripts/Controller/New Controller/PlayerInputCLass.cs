using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace AJK
{
    public class PlayerInputCLass
    {
        private PlayerControls _controls;

        private void OnSceneUnload(Scene current)
        {
            SprintToggle = null;
            DodgeAction = null;
            SpecialAction = null;
            AttackAction = null;
            HintAction = null;
            InteractAction = null;
            Special1 = null;
            Special2 = null;
            Special3 = null;

            SceneManager.sceneUnloaded -= OnSceneUnload;
        }

        public PlayerInputCLass()
        {
            _controls = new PlayerControls();
            _controls.Interaction.Enable();
            // _controls.UI.Disable();

            _controls.Main.Move.performed += GetMoveVector;
            _controls.Main.Sprint.started += ToggleSprint;
            _controls.Main.CameraReset.started += CamRentre;

            _controls.PlayerActions.Dodge.started += DodgeEvent;
            _controls.PlayerActions.SpecialAttack.started += SpecialAttackEvent;
            _controls.PlayerActions.MainAttack.started += AttackEvent;
            _controls.PlayerActions.Hint.started += HintEvent;
            _controls.PlayerActions.Prime.started += PrimeEvent;
            _controls.PlayerActions.Prime.canceled += PrimeEvent;

            _controls.Interaction.Interact.started += InteractEvent;

            _controls.SpecialAttackMap.Special1.started += SpecialAttack1;
            _controls.SpecialAttackMap.Special2.started += SpecialAttack2;
            _controls.SpecialAttackMap.Special3.started += SpecialAttack3;

            _controls.PlayerActions.PrimedSpecial1.started += PrimedSpecialAttack1;
            _controls.PlayerActions.PrimedSpecial2.started += PrimedSpecialAttack2;
            _controls.PlayerActions.PrimedSpecial3.started += PrimedSpecialAttack3;

            _controls.Main.Pause.started += Menu;
            _controls.UI.Back.started += Exit;

            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        public void EnableMainInput()
        {
            _controls.Main.Enable();
        }

        public void DisableMainInput()
        {
            _controls.Main.Disable();
        }

        public void EnableMainPlayerActions()
        {
            _controls.PlayerActions.Enable();
        }

        public void DisableMainPlayerActions()
        {
            _controls.PlayerActions.Disable();
        }

        public void EnableUIInput()
        {
            _controls.UI.Enable();
        }

        public void DisableUIInput()
        {
            _controls.UI.Disable();
        }

        public void EnableSpecialAttacks()
        {
            _controls.SpecialAttackMap.Enable();
        }

        public void DisableSpecialAttacks()
        {
            _controls.SpecialAttackMap.Disable();
        }

        public Vector3 inputVector { get; private set; }
        public float moveAmount;
        public void GetMoveVector(InputAction.CallbackContext context)
        {
            var inVec = context.ReadValue<Vector2>();
            inputVector = new Vector3(inVec.x, 0f, inVec.y);
            moveAmount = Mathf.Clamp01(Mathf.Abs(inVec.x) + Mathf.Abs(inVec.y));
        }

        public Action SprintToggle;
        public void ToggleSprint(InputAction.CallbackContext context)
        {
            if (specialAttackPrimed)
                return;

            SprintToggle?.Invoke();
        }

        public Action DodgeAction;
        public void DodgeEvent(InputAction.CallbackContext context)
        {
            DodgeAction?.Invoke();
        }

        public Action SpecialAction;
        public void SpecialAttackEvent(InputAction.CallbackContext context)
        {
            if (specialAttackPrimed)
            {
                return;
            }
            SpecialAction?.Invoke();
        }

        public Action AttackAction;
        public void AttackEvent(InputAction.CallbackContext context)
        {
            if (specialAttackPrimed)
            {
                return;
            }
            AttackAction?.Invoke();
        }

        public Action HintAction;
        public void HintEvent(InputAction.CallbackContext context)
        {
            HintAction?.Invoke();
        }

        public Action InteractAction;
        public void InteractEvent(InputAction.CallbackContext context)
        {
            InteractAction?.Invoke();
        }

        public bool specialAttackPrimed = false;
        private void PrimeEvent(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableSpecialAttacks();
                    specialAttackPrimed = true;
                    break;
                case InputActionPhase.Canceled:
                    DisableSpecialAttacks();
                    specialAttackPrimed = false;
                    break;
                default:
                    break;
            }
        }

        public Action Special1, Special2, Special3;
        private void SpecialAttack1(InputAction.CallbackContext context)
        {
            if (!specialAttackPrimed)
            {
                return;
            }

            Special1?.Invoke();
        }

        private void SpecialAttack2(InputAction.CallbackContext context)
        {
            if (!specialAttackPrimed)
            {
                return;
            }

            Special2?.Invoke();
        }

        private void SpecialAttack3(InputAction.CallbackContext context)
        {
            if (!specialAttackPrimed)
            {
                return;
            }

            Special3?.Invoke();
        }

        private void PrimedSpecialAttack1(InputAction.CallbackContext context)
        {
            Special1?.Invoke();
        }

        private void PrimedSpecialAttack2(InputAction.CallbackContext context)
        {

            Special2?.Invoke();
        }

        private void PrimedSpecialAttack3(InputAction.CallbackContext context)
        {
            Special3?.Invoke();
        }

        public Action OpenMenu;
        private void Menu(InputAction.CallbackContext context)
        {
            OpenMenu?.Invoke();
        }

        public Action Back;
        public void Exit(InputAction.CallbackContext context)
        {
            Back?.Invoke();
            Debug.Log("Back");
        }

        public Action Recentre;
        private void CamRentre(InputAction.CallbackContext context)
        {
            Recentre?.Invoke();
        }
    }
}

