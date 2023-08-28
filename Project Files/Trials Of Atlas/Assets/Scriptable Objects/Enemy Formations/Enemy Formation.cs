using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "SO's/Enemies/Formation")]
public class EnemyFormation : ScriptableObject
{
    [Serializable]
    public class Formation
    {
        public GameObject formationPrefab;
        public float formationValue;
    }
    
    [SerializeField]public List<Formation> formations;

    private void OnValidate()
    {
        foreach (var form in formations)
        {
            if (form.formationPrefab == null)
                continue;
            form.formationValue = form.formationPrefab.transform.childCount;
        }
    }
}
