using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class BitCurrencyHandler : MonoBehaviour
{
    public PlayerSO stats;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickupBits(int amount)
    {
        stats.PickupBits(amount);
    }
}
