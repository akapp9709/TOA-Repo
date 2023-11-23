using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO's/Special Attack")]
public class SpecialAttack : ScriptableObject
{
    public string triggerName;
    public float damageMultiplier;
    public float CoolDown = 5f;
}
