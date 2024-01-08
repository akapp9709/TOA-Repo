using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Bounty")]
public class Bounty : ScriptableObject
{
    public enum enemyType
    {
        Any,
        Creep,
        Range,
        Brute
    }
    public enum Type
    {
        Cull,
        Collect,
        Complete_Rooms
    }
    public Type type;

    public int quantity = 3;
    public int reward = 10;
    public enemyType enemy = enemyType.Any;
}

[CustomEditor(typeof(Bounty))]
public class BountyEditor : Editor
{
}
