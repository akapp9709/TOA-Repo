using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        public bool hasStamina, canUseStamina;

        public static PlayerManager singleton;
        // Start is called before the first frame update
        void Awake()
        {
            singleton = this;
            maxHealth = playerStats.Health;
            currentHealth = maxHealth;
            maxStamina = playerStats.Stamina;
            currentStamina = maxStamina;
            strength = playerStats.Strength;
            defense = playerStats.Vitality;
            staminaRegen = playerStats.StaminaRegen;
        }

        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _animator = GetComponentInChildren<Animator>();

            GetComponentInChildren<HitBox>().SetupValues("Enemy", strength);

            _inputHandler.OnDodgeEvent += Dodge;
            canUseStamina = true;

            _inputHandler.Interact += GainEffect;
        }

        // Update is called once per frame
        void Update()
        {


            if (_inputHandler.isSprinting)
            {
                currentStamina -= playerStats.staminaDepletion * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
            else
            {
                currentStamina += staminaRegen * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }

            if (currentStamina < 1)
            {
                StartCoroutine(StaminaCoolDown());
                _inputHandler.isSprinting = false;
            }

            _inputHandler.isInteracting = _animator.GetBool("IsInteracting");
            _inputHandler.dodgeFlag = false;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= (damage - defense);
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
            yield return new WaitForSeconds(3f);
            canUseStamina = true;
        }

        public void GainEffect()
        {
            Reward rew = FindAnyObjectByType<Reward>();

            if (Vector3.Distance(rew.transform.position, transform.position) > 2f)
                return;

            switch (rew.type)
            {
                case Reward.RewardType.Heal:
                    currentHealth += rew.amount;
                    break;
                case Reward.RewardType.DamageUp:
                    strength += rew.amount;
                    GetComponentInChildren<HitBox>().UpdateValue(strength);
                    break;
                case Reward.RewardType.DefenseUp:
                    defense += rew.amount;
                    break;
            }

            rew.PickUp();
        }
    }
}
