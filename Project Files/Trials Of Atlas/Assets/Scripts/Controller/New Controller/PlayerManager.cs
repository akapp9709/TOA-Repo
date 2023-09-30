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
        [HideInInspector] public float maxHealth, currentHealth;
        [HideInInspector] public float defense;
        [HideInInspector] public float strength;
        [HideInInspector] public float maxStamina, currentStamina, staminaRegen;
        [HideInInspector] public bool hasStamina, canUseStamina;

        public static PlayerManager singleton;
        // Start is called before the first frame update
        void Awake()
        {
            singleton = this;
            maxHealth = playerStats.Health;
            maxStamina = playerStats.Stamina;
            strength = playerStats.Strength;
            defense = playerStats.Vitality;
            staminaRegen = playerStats.StaminaRegen;
        }

        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _animator = GetComponentInChildren<Animator>();

            GetComponentInChildren<HitBox>().SetupValues("Enemy", strength);
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            _inputHandler.OnDodgeEvent += Dodge;
        }

        // Update is called once per frame
        void Update()
        {
            currentStamina += staminaRegen * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina < 1)
            {
                hasStamina = false;
            }

            _inputHandler.isInteracting = _animator.GetBool("IsInteracting");
            _inputHandler.dodgeFlag = false;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage - defense;
        }

        public void UseStamina(float amount)
        {
            currentStamina -= amount;
        }

        private void Dodge()
        {
            UseStamina(playerStats.dodgeCost);
        }

        private IEnumerator StaminaCoolDown()
        {
            canUseStamina = false;
            yield return new WaitForSeconds(1f);
            canUseStamina = true;
        }
    }
}
