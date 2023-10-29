using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        private static PlayerManager singleton;
        public static PlayerManager Singleton
        {
            get { return singleton; }
            private set { singleton = value; }
        }


        public Action PlayerDeath;
        public Action StateChange;

        private float strMod, defMod;

        private void OnSceneUnload(Scene current)
        {
            PlayerDeath = null;
            StateChange = null;
            playerStats.StatChange = null;
        }
        // Start is called before the first frame update
        void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else if (this != singleton)
            {
                Destroy(gameObject);
            }

            playerStats.StatChange += UpdateStats;
            maxHealth = playerStats.Health;
            currentHealth = maxHealth;
            maxStamina = playerStats.Stamina;
            currentStamina = maxStamina;
            strength = playerStats.Strength;
            defense = playerStats.Vitality;
            staminaRegen = playerStats.StaminaRegen;

            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _animator = GetComponentInChildren<Animator>();

            GetComponentInChildren<HitBox>().SetupValues("Enemy", strength);

            _inputHandler.OnDodgeEvent += Dodge;
            canUseStamina = true;

            _inputHandler.Interact += GainEffect;

            //GameManager.Singleton.DungeonClear += playerStats.CompleteRun;
        }

        // Update is called once per frame
        void Update()
        {
            if (_inputHandler.isSprinting)
            {
                UseStamina(playerStats.staminaDepletion * Time.deltaTime);
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
            StateChange?.Invoke();
        }

        private void UpdateStats()
        {
            strength = playerStats.Strength * (1 + strMod);
            defense = playerStats.Vitality * (1 + defMod);
            maxStamina = playerStats.Stamina;
            staminaRegen = playerStats.StaminaRegen;
            maxHealth = playerStats.Health;

            GetComponentInChildren<HitBox>().UpdateValue(strength);
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= (damage - defense);
            playerStats.GainXP(PlayerSO.Skill.Vitality, damage);

            if (currentHealth <= 0)
            {
                Debug.Log("Player has Died");
                PlayerDeath?.Invoke();
            }
        }

        public void UseStamina(float amount)
        {
            currentStamina -= amount;
            playerStats.GainXP(PlayerSO.Skill.Endurance, amount);
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
            if (FindAnyObjectByType<Reward>() == null)
                return;
            Reward rew = FindAnyObjectByType<Reward>();

            if (Vector3.Distance(rew.transform.position, transform.position) > 2f)
                return;

            switch (rew.type)
            {
                case Reward.RewardType.Heal:
                    currentHealth += (rew.amount / 100) * maxHealth;
                    break;
                case Reward.RewardType.DamageUp:
                    strMod += rew.amount / 100;
                    break;
                case Reward.RewardType.DefenseUp:
                    defMod += rew.amount / 100;
                    break;
            }
            UpdateStats();
            rew.PickUp();
        }
    }
}
