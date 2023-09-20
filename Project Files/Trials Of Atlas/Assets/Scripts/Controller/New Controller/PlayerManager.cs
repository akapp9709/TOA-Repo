using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AJK
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler _inputHandler;

        private Animator _animator;

        public PlayerSO playerStats;
        private float maxHealth;
        private float defense;
        private float strength;
        private float maxStamina;
        // Start is called before the first frame update
        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _animator = GetComponentInChildren<Animator>();

            GetComponentInChildren<HitBox>().SetupValues("Enemy", 10f);
        }

        // Update is called once per frame
        void Update()
        {
            _inputHandler.isInteracting = _animator.GetBool("IsInteracting");
            _inputHandler.dodgeFlag = false;
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"I have taken {damage} points of damage");
        }
    }
}
