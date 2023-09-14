using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBrain : EnemyFSM
{
    public EnemyStats enemyStats;
    protected Dictionary<string, object> knowledge = new Dictionary<string, object>();

    public void AddToDictionary(string name, object value)
    {
        if(knowledge.ContainsKey(name))
            return;

        knowledge[name] = value;
    }

    public object GetValue(string name)
    {
        if (knowledge.ContainsKey(name))
        {
            return knowledge[name];
        }
        else
            return null;
    }
}
