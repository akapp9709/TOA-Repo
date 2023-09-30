using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player")]
public class PlayerSO : ScriptableObject
{
    public float walkSpeed = 3;
    public float runSpeed = 3;
    public float sprintSpeed = 7;

    public float maxStamina = 25;
    public float staminaRegen = 10;
    public float staminaDepletion = 20;
    public int staminaLevel = 1;

    public int runCompleted = 0;

    public float dodgeSpeed = 8f;
    public float dodgeTime = 1f;
    public float dodgeCost = 10f;

    public float health = 100;
    public int healthLevel = 1;
    public float strength = 20;
    public int strengthLevel = 1;
    public float defense = 3;
    public int defenseLevel = 1;

    public float Strength
    {
        get
        {
            var s = strength + Mathf.Pow(2f, strengthLevel / 2.7f);
            s *= 100;
            s = Mathf.RoundToInt(s);
            s /= 100;
            return s;
        }
    }

    public float Health
    {
        get
        {
            var h = health + (healthLevel * 10);
            return h;
        }
    }

    public float Vitality
    {
        get
        {
            var v = defense + Mathf.Log(defenseLevel, 1.15f);
            v *= 100;
            v = Mathf.RoundToInt(v);
            v /= 100;
            return v;
        }
    }

    public float Stamina
    {
        get
        {
            var s = maxStamina + (staminaLevel * 5);
            return s;
        }
    }

    public float StaminaRegen
    {
        get
        {
            var sr = Stamina / 10f - 2;
            sr *= 100;
            sr = Mathf.RoundToInt(sr);
            sr /= 100;
            return sr;
        }
    }
}
