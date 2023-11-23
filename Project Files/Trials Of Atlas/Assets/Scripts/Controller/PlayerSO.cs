using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player")]
public class PlayerSO : ScriptableObject
{
    public enum Skill
    {
        Strength,
        Health,
        Vitality,
        Endurance,
        Dexterity
    }

    [Header("Main Stats")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 6;
    public float sprintSpeed = 10;
    public float maxStamina = 25;
    public float health = 100;
    public float strength = 10;
    public float attackRange = 6;
    public float defense = 3;
    public int runCompleted = 0;
    public int bitCollected = 0;
    public int purseSize = 0;
    public Action GainBits;

    [Header("Levels")]
    public int staminaLevel = 1;
    public float staminaExp = 0, sNeeded;
    public int strengthLevel = 1;
    public float strengthExp = 0, dONeeded;
    public int defenseLevel = 1;
    public float defenseExp = 0, dINeeded;
    public int healthLevel = 1;
    public int healthXP = 0;
    public int dexLevel = 1;
    public float dexExp = 0, dexNeeded;

    [Header("Sub Stats")]
    public float staminaRegen = 0;
    public float staminaDepletion = 5f;
    public float dodgeSpeed = 15f;
    public float dodgeTime = 0.3f;
    public float dodgeCost = 10f;

    public Action StatChange;

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

    public void PickupBits(int amount)
    {
        bitCollected += amount;
        GainBits?.Invoke();
    }

    private void GainHealthXP()
    {
        healthXP++;
        if (healthXP % 4 == 0)
        {
            healthLevel++;
            StatChange?.Invoke();
            healthXP = 0;
        }
    }

    public void GainXP(Skill skill, float amount)
    {
        switch (skill)
        {
            case Skill.Strength:
                strengthExp += amount;
                if (strengthExp > StrengthExpRequired)
                {
                    strengthLevel++;
                    strengthExp = 0;
                    StatChange?.Invoke();
                    GainHealthXP();
                }
                break;
            case Skill.Health:
                break;
            case Skill.Vitality:
                defenseExp += amount;
                if (defenseExp > DefenseExpRequired)
                {
                    defenseLevel++;
                    defenseExp = 0;
                    StatChange?.Invoke();
                    GainHealthXP();
                }
                break;
            case Skill.Endurance:
                staminaExp += amount;
                if (staminaExp > EnduranceExpRequired)
                {
                    staminaLevel++;
                    staminaExp = 0;
                    StatChange?.Invoke();
                    GainHealthXP();
                }
                break;
        }
    }

    private float StrengthExpRequired
    {
        get
        {
            dONeeded = Strength * (1 + (5 * Strength));
            return dONeeded;
        }
    }

    private float DefenseExpRequired
    {
        get
        {
            dINeeded = Mathf.Pow(Vitality * (1 + (Vitality / 2)), 1.5f);
            return dINeeded;
        }
    }

    private float EnduranceExpRequired
    {
        get
        {
            sNeeded = Stamina * (1 + (Stamina / staminaDepletion));
            return sNeeded;
        }
    }

    public void CompleteRun()
    {
        runCompleted++;
    }
}
