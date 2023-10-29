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
    [SerializeField] private RectTransform gameOverScreen;
    [SerializeField] private RectTransform victoryScreen;

    private PlayerManager playerManager;
    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += InitilaizeUI;
    }

    private void InitilaizeUI()
    {
        playerManager = PlayerManager.Singleton;
        loadingScreen.gameObject.SetActive(false);
        isReady = true;
        playerManager.PlayerDeath += GameOver;
        GameManager.Singleton.DungeonClear += Victory;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
            return;
    }

    private void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
    }

    private void Victory()
    {
        victoryScreen.gameObject.SetActive(true);
    }
}
