using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AJK
{
    public class AnimationHandler : MonoBehaviour
    {
        private Animator _anim;
        private int _vertical, _horizontal;
        public bool canRotate;
        
        public void Initialize()
        {
            _anim = GetComponent<Animator>();
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
            else if (vertical is >0.55f and <= 1f)
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
            #endregion
            
            #region Horizontal
            float h = 0;

            if (horizontal is > 0 and < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontal is >0.55f and <= 1f)
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
            
            _anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            _anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
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
