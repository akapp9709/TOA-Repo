using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO's/Path")]
public class Path : ScriptableObject
{
    public string PathName;
    public float bias;
    [SerializeField] private List<RewardData> rewards;
}
