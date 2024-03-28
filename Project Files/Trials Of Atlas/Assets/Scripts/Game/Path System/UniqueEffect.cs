using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(menuName = "SO's/UniqueEffect", order = 0)]
public class UniqueEffect : ScriptableObject 
{
    public string componentName;
    public CombatEventManager.CombatEventType CombatEvent;
}



