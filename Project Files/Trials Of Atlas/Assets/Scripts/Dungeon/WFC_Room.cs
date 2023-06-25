using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_Room : MonoBehaviour
{
    public enum Direction
    {
        North,
        South,
        East,
        West,
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    }

    public enum RoomTypes
    {
        Standard,
        Combat,
        Agility,
        Stairs
    }
    
    [SerializeField] private Transform exit;
    [SerializeField] public List<Direction> unavailableDirections;
    public Direction roomDirection, exitDirection;
    public RoomTypes roomType;
    public List<RoomTypes> prohibitedRooms;

    public Vector3 exitPosition => exit.position;
    public Vector3 exitForward => exit.eulerAngles;
    public Vector3 roomPosition => transform.position;

    public Transform roomCentre;
    public float checkRadius, dist;
    public Vector3 dir, penePos;

    private bool clipping;
    
    public bool CheckForOverlap()
    {
        int layer = LayerMask.NameToLayer("Room");
        bool clear;
        var coll = GetComponent<BoxCollider>();
        var pos = transform.position;
        var rot = transform.rotation;
        var dim = coll.size;
        
        var colls = Physics.OverlapBox(roomCentre.position, dim, Quaternion.identity , ~layer);
        foreach (var x in colls)
        {
            if (x.GetComponent<WFC_Room>() == null) continue;
            if (Physics.ComputePenetration(coll, roomCentre.position, roomCentre.rotation, x, 
                    x.transform.position,
                    x.transform.rotation, out dir, out dist) 
                && x.transform != this.transform)
            {
                if (dist > 0.3f)
                {
                    clipping = true;
                    penePos = x.transform.position;
                    Debug.Log(x.name + " " + this.name + " : " + dist + " : " + dir);
                    return false;
                }
                else
                {
                    clipping = false;
                }
            }
        }

        return true;
    }
}
