using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PF_Room : MonoBehaviour
{
    public Vector3 roomSize, roomSizeOffset;
    public List<Transform> entrancePositions = new List<Transform>();
    public Transform pivotCorner;
    public Transform firstCorner, secondCorner;
    public List<GameObject> roomCorners;

    private float runTimes;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            roomSizeOffset + transform.position
            , roomSize
        );

        foreach (var x in roomCorners)
        {
            Gizmos.DrawWireSphere(x.transform.position, 0.2f);
        }

        foreach (var pos in entrancePositions)
        {
            Gizmos.DrawSphere(pos.position, 1f);
        }
    }

    private void Awake()
    {
        FindRoomCorners();
    }

    private void FindRoomCorners()
    {
        var arr = GameObject.FindGameObjectsWithTag("RoomCorner");

        foreach (var x in arr)
        {
            if(x.transform.IsChildOf(this.transform))
                roomCorners.Add(x);
        }
    }

    public Vector2Int Offset2D
    {
        get
        {
            var vec = Vector2Int.FloorToInt(new Vector2(
                roomSizeOffset.x + transform.position.x, 
                roomSizeOffset.z + transform.position.z));
            return vec;
        }
    }
    
    public Vector2Int Size2D
    {
        get
        {
            var vec = Vector2Int.FloorToInt(new Vector2(roomSize.x, roomSize.z));
            return vec;
        }
    }

    public Vector2Int Location2D
    {
        get
        {
            var vec = Vector2Int.FloorToInt(new Vector2(
                pivotCorner.position.x, 
                pivotCorner.position.z));
            return vec;
        }
    }
}
