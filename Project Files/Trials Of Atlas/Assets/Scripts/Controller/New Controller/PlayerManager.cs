using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AJK
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler _inputHandler;

        private Animator _animator;
        // Start is called before the first frame update
        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _animator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            _inputHandler.isInteracting = _animator.GetBool("IsInteracting");
            _inputHandler.dodgeFlag = false;
        }
    }
}
