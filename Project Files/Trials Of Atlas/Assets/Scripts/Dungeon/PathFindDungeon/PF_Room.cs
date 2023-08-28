using System.Collections.Generic;
using UnityEngine;

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
        // Gizmos.DrawWireCube(roomSizeOffset, roomSize);
    }

    private void Awake()
    {
        FindRoomCorners();
        roomSize = GetComponent<BoxCollider>().size;
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
            var vec = new Vector2Int(Mathf.RoundToInt(roomSize.x), Mathf.RoundToInt(roomSize.z));
            return vec;
        }
    }

    public Vector2Int Location2D
    {
        get
        {
            var vec = new Vector2Int(
                Mathf.RoundToInt(pivotCorner.position.x),
                Mathf.RoundToInt(pivotCorner.position.z)
                );
            return vec;
        }
    }

    private void MarkSpace()
    {
        var rectangle = new Rect(Location2D, Size2D);
        
    }
}
