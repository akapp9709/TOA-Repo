using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO's/Enemy")]
public class EnemyStats : ScriptableObject
{
    public float strength;
    public float health;
    [Range(0, 1)] public float aggression;
    public float attackRange;
    public float safetyBufferRadius;
    public float maxMoveDistance = 9;
    public float avoidance = 0.8f;

    [Serializable]
    public struct BitDrop
    {
        public int amount;
        public float chance;
    }

    [SerializeField] public List<BitDrop> bitDrops;

    public int DropBits()
    {
        int output = 0;

        var rand = UnityEngine.Random.Range(0, BiasTotal);
        float chanceOffset = 0;
        for (int i = 0; i < bitDrops.Count; i++)
        {
            float x = bitDrops[i].chance;
            if (rand > x + chanceOffset)
            {
                chanceOffset += x;
                continue;
            }

            output = bitDrops[i].amount;
            break;
        }

        return output;
    }

    float BiasTotal
    {
        get
        {
            float total = 0;

            foreach (var drop in bitDrops)
            {
                total += drop.chance;
            }
            return total;
        }
    }
}
