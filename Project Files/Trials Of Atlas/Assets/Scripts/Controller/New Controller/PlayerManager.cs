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
        public PlayerInputCLass inputHandler;

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

        private PlayerLocomotion _move;


        public Action PlayerDeath;
        public Action StateChange;

        private float strMod, defMod;

        private bool _canTakeDamage = true;

        private void OnSceneUnload(Scene current)
        {
            PlayerDeath = null;
            StateChange = null;
            playerStats.StatChange = null;
            inputHandler = null;
            SceneManager.sceneUnloaded -= OnSceneUnload;
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
            inputHandler = new PlayerInputCLass();

            playerStats.StatChange += UpdateStats;
            maxHealth = playerStats.Health;
            currentHealth = maxHealth;
            maxStamina = playerStats.Stamina;
            currentStamina = maxStamina;
            strength = playerStats.Strength;
            defense = playerStats.Vitality;
            staminaRegen = playerStats.StaminaRegen;

            SceneManager.sceneUnloaded += OnSceneUnload;
            _move = GetComponent<PlayerLocomotion>();
        }

        void Start()
        {
            inputHandler.EnableMainInput();
            inputHandler.EnableMainPlayerActions();
            _animator = GetComponentInChildren<Animator>();

            GetComponentInChildren<HitBox>().SetupValues("Enemy", strength);

            canUseStamina = true;

            inputHandler.DodgeAction += Dodge;

            GameManager.Singleton.DungeonClear += playerStats.CompleteRun;
        }

        public void InitializePlayerControls()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_move.isSprinting)
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
                _move.isSprinting = false;
            }

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
            if (!_canTakeDamage)
                return;

            _canTakeDamage = false;
            StartCoroutine(ResetTriggerTimer());


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
            if (!canUseStamina)
                return;

            UseStamina(playerStats.dodgeCost);
        }

        private IEnumerator StaminaCoolDown()
        {
            canUseStamina = false;
            yield return new WaitForSeconds(3f);
            canUseStamina = true;
        }

        public void GainEffect(Reward rew)
        {
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

        private IEnumerator ResetTriggerTimer()
        {
            yield return new WaitForSeconds(0.1f);
            _canTakeDamage = true;
        }
    }
}
