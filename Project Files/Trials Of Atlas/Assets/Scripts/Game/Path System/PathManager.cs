using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager
{
    
}

[System.Serializable]
public class RewardData
{
    public enum Type
    {
        DamageUp,
        DefenseUp,
        HealthRegen,
        Unique
    }

    public Type rewardType = Type.DamageUp;
    public float chance;
    public UniqueEffect effect;
}