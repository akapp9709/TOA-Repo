using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AJK
{
    public class AnimationHandler : MonoBehaviour
    {
        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion playerMove;
        private int _vertical, _horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerMove = GetComponentInParent<PlayerLocomotion>();
            _vertical = Animator.StringToHash("Vertical");
            _horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float vertical, float horizontal)
        {
            #region Vertical

            float v = 0;

            if (vertical is > 0 and < 0.55f)
            {
                v = 0.5f;
            }
            else if (vertical is > 0.55f and <= 1f)
            {
                v = 1f;
            }
            else if (vertical is < 0 and > -0.55f)
            {
                v = -0.5f;
            }
            else if (vertical is < 0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0f;
            }

            if (inputHandler.isSprinting)
            {
                v *= 2;
            }
            #endregion

            #region Horizontal
            float h = 0;

            if (horizontal is > 0 and < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontal is > 0.55f and <= 1f)
            {
                h = 1f;
            }
            else if (horizontal is < 0 and > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontal is < 0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0f;
            }
            #endregion

            anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string animClip, bool interacting)
        {
            anim.applyRootMotion = interacting;
            anim.SetBool("IsInteracting", interacting);
            anim.CrossFade(animClip, 0.2f);
        }

        public void TriggerTargetAnimation(string trigger, bool interacting)
        {
            anim.applyRootMotion = interacting;
            anim.SetBool("IsInteracting", interacting);
            anim.SetTrigger(trigger);
        }

        public void SetAnimationValuesBool(string variable, bool value)
        {
            anim.SetBool(variable, value);
        }

        private void OnAnimatorMove()
        {
            if (inputHandler.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            playerMove._rb.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerMove._rb.velocity = velocity;
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }
    }
}
