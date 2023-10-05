using System.Collections;
using System.Collections.Generic;
using AJK;
using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform loadingScreen;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform staminaBar;
    [SerializeField] private TextMeshProUGUI attackStat;
    [SerializeField] private TextMeshProUGUI defenseStat;

    private PlayerManager playerManager;
    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += InitilaizeUI;
    }

    private void InitilaizeUI()
    {
        playerManager = PlayerManager.singleton;
        loadingScreen.gameObject.SetActive(false);
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
            return;
        UpdateAttackValue();
        UpdateDefenseValue();
        UpdateHealthValue();
        UpdateStaminaValue();
    }

    private void UpdateAttackValue()
    {
        attackStat.SetText($"Attack: {playerManager.strength}");
    }

    private void UpdateHealthValue()
    {
        healthBar.GetComponent<ProgressBar>().SetValues(playerManager.currentHealth / playerManager.maxHealth);
    }

    private void UpdateStaminaValue()
    {
        staminaBar.GetComponent<ProgressBar>().SetValues(playerManager.currentStamina / playerManager.maxStamina);
    }
    private void UpdateDefenseValue()
    {
        defenseStat.SetText($"Defense: {playerManager.defense}");
    }
}
