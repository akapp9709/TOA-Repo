using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public enum RewardType
    {
        Heal,
        DamageUp,
        DefenseUp
    }
    public RewardType type;

    public float amount;

    public void PickUp()
    {
        Destroy(this.gameObject);
    }
}
