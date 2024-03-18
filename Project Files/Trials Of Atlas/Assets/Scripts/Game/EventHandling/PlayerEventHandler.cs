using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEventHandler : MonoBehaviour
{
    public CombatEventManager cm = new CombatEventManager();

    public void AddAction(CombatEventManager.CombatEventType eventType, UniqueEffect effect)
    {
        switch (eventType)
        {
            case CombatEventManager.CombatEventType.AttackStart:
                var comp = gameObject.AddComponent(Type.GetType(effect.componentName)) as AttackStartEvent;
                cm.AttackStart += comp.EventAction;
                break;
            case CombatEventManager.CombatEventType.LastAttackStart:
                break;
            case CombatEventManager.CombatEventType.LastAttackEnd:
                break;
            case CombatEventManager.CombatEventType.DashStart:
                var compDS = gameObject.AddComponent(Type.GetType(effect.componentName)) as DashStartEvent;
                cm.AttackStart += compDS.EventAction;
                break;
            case CombatEventManager.CombatEventType.DashEnd:
                var compDE = gameObject.AddComponent(Type.GetType(effect.componentName)) as DashEndEvent;
                cm.AttackStart += compDE.EventAction;
                break;
            default:
                break;
        }
    }
}
