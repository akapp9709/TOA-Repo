using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player")]
public class PlayerSO : ScriptableObject
{
    public float runSpeed = 3;
    public float sprintSpeed = 7;
    
    public float maxStamina = 100;
    public float staminaRegen = 10;
    public float staminaDepletion = 20;
}
