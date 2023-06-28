using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PF_Generator : MonoBehaviour
{
    public enum CellType
    {
        None,
        Buffer,
        Room,
        Hallway
    }
    
    [SerializeField] private List<GameObject> roomPrefabs;
    private PF_Grid<CellType> _grid;
    public Vector2Int maxSize, offset, roomBuffer;
    public int dungeonLength;
    private List<Vector3> _testPoints = new List<Vector3>();
    private List<RectInt> _testRects = new List<RectInt>();
    private List<RectInt> _testBuffer = new List<RectInt>();

    private Vector2Int _rectOffset;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var offPos = new Vector3(offset.x, 0f, offset.y);
        var gridSize = new Vector3(maxSize.x, 0f, maxSize.y);
        Gizmos.DrawWireCube(gridSize/2, gridSize);

        foreach (var x in _testPoints)
        {
            Gizmos.DrawWireSphere(x, 0.4f);
        }
        
        Gizmos.color = Color.yellow;
        foreach (var x in _testBuffer)
        {
            Gizmos.DrawSphere(new Vector3(x.position.x, 0f, x.position.y), 0.5f);
            foreach (var y in x.allPositionsWithin)
            {
                var pos = new Vector3(y.x, 0f, y.y);
                Gizmos.DrawCube(pos, 0.4f*Vector3.one);
            }
        }
        
        Gizmos.color = Color.blue;
        foreach (var x in _testRects)
        {
            Gizmos.DrawSphere(new Vector3(x.position.x, 0f, x.position.y), 0.5f);
            foreach (var y in x.allPositionsWithin)
            {
                var pos = new Vector3(y.x, 0f, y.y);
                Gizmos.DrawCube(pos, 0.4f*Vector3.one);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _grid = new PF_Grid<CellType>(maxSize, offset);
        _rectOffset = new Vector2Int(1, 0);
        
        PlaceRooms();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < dungeonLength; i++)
        {
            var pos = new Vector3(
                Random.Range(0, (int)maxSize.x + 1),
                0f,
                Random.Range(0, (int)maxSize.y + 1)
            );

            var ind = Random.Range(0, roomPrefabs.Count);

            var room = Instantiate(roomPrefabs[ind], pos, Quaternion.identity);
            ChooseRoomRotation(room);
            
            var roomVar = room.GetComponent<PF_Room>();
            var roomRect = new RectInt(roomVar.Location2D, roomVar.Size2D + Vector2Int.one);
            var roomBufferRect = new RectInt(roomRect.position - _rectOffset, roomRect.size + _rectOffset*2);

            bool safe = true;
            var inBox = CheckRoomInBounds(roomBufferRect);
            
            if (!inBox)
            {
                safe = false;
            }
            else if(!CheckRoomIntersect(roomBufferRect))
            {
                safe = false;
            }

            if (safe)
            {
                _testBuffer.Add(roomBufferRect);
                _testRects.Add(roomRect);
                FinalizeRoom(roomRect);
                Debug.Log("No Problems here");
            }
            else
            {
                Destroy(room);
                i--;
                Debug.Log("Resetting");
            }
        }
    }

    private void ChooseRoomRotation(GameObject room)
    {
        var rotVal = Random.Range(1, 5);
        var roomVar = room.GetComponent<PF_Room>();
        var currentSize = room.GetComponent<PF_Room>().roomSize;
        var currentOffset = room.GetComponent<PF_Room>().roomSizeOffset;
        var newSize = new Vector3();
        switch (rotVal)
        {
            case 1:
                newSize = currentSize;
                roomVar.pivotCorner = roomVar.firstCorner;
                _rectOffset = new Vector2Int(roomBuffer.x, roomBuffer.y);
                break;
            case 2:
                room.transform.Rotate(Vector3.up, 90);
                newSize = new Vector3(currentSize.z, currentSize.y, currentSize.x);
                currentOffset = newSize / 2;
                currentOffset.z = 0;
                roomVar.pivotCorner = roomVar.secondCorner;
                _rectOffset = roomBuffer;
                break;
            case 3:
                room.transform.Rotate(Vector3.up, 180);
                newSize = new Vector3(currentSize.x, currentSize.y, currentSize.z * -1);
                currentOffset = newSize / 2;
                currentOffset.x = 0;
                roomVar.pivotCorner = roomVar.secondCorner;
                _rectOffset = new Vector2Int(roomBuffer.x, roomBuffer.y * -1);
                break;
            case 4:
                room.transform.Rotate(Vector3.up, 270);
                newSize = new Vector3(currentSize.z * -1, currentSize.y, currentSize.x);
                currentOffset = newSize / 2;
                currentOffset.z = 0;
                roomVar.pivotCorner = roomVar.firstCorner;
                _rectOffset = new Vector2Int(roomBuffer.x * -1, roomBuffer.y);
                break;
        }

        room.GetComponent<PF_Room>().roomSize = newSize;
        room.GetComponent<PF_Room>().roomSizeOffset = currentOffset;
    }
    
    private bool CheckRoomInBounds(RectInt area)
    {
        foreach (var x in area.allPositionsWithin)
        {
            if (!_grid.InBounds(x))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckRoomIntersect(RectInt area)
    {
        foreach (var vec in area.allPositionsWithin)
        {
            if (_grid[vec] == CellType.Room && _grid.InBounds(vec))
            {
                Debug.Log("Intersection Present");
                return false;
            }
        }

        return true;
    }

    private void FinalizeRoom(RectInt area)
    {
        foreach (var pos in area.allPositionsWithin)
        {
            _grid[pos] = CellType.Room;
        }
    }
}
