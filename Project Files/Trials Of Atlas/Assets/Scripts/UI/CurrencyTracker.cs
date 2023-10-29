using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyTracker : MonoBehaviour
{
    public PlayerSO playerStats;
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        playerStats.GainBits = UpdateValue;
        text = GetComponent<TextMeshProUGUI>();
    }

    private void UpdateValue()
    {
        text.text = $"Bits: {playerStats.bitCollected}";
    }
}
