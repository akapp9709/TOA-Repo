using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SO's/Dungeon Asset Pack")]
public class AssetPack : ScriptableObject
{
    public GameObject floor, corner, wall;
    public Material floorMat, wallMat;
}
