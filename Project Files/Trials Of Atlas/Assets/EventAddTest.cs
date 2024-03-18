using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventAddTest : MonoBehaviour
{
    public UniqueEffect testEffect;
    // Start is called before the first frame update
    public void AddCombatEvent(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;
        GameObject.FindObjectOfType<PlayerEventHandler>().AddAction(testEffect.CombatEvent, testEffect);
    }
}
