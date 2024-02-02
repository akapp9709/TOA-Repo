using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public GameObject Object {get; private set;}
    public Vector2 Boundary {get; private set;}
    public Vector3 Position {get; private set;}
    public RoomData(GameObject obj, Vector2 bounds, Vector3 position)
    {
        Object = obj;
        Boundary = bounds;
        Position = position;
    }
}
