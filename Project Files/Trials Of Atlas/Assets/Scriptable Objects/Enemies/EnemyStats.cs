using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO's/Enemy")]
public class EnemyStats : ScriptableObject
{
    public float strength;
    public float health;
    [Range(0, 1)]public float aggression;
}