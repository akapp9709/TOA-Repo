using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(DungeonBuilder))]
public class GridBuilder : MonoBehaviour
{
    public Action GridComplete;
    public enum CellType
    {
        None,
        Buffer,
        Room,
        Hallway,
        WallEast,
        WallWest,
        WallNorth,
        WallSouth,
        CornerOutNE,
        CornerOutNW,
        CornerOutSE,
        CornerOutSW,
        CornerInNE,
        CornerInNW,
        CornerInSE,
        CornerInSW
    }
    public PF_Grid<CellType> grid {get; private set;}
    public Vector2Int maxSize, padding, offset, buffer;
    private Vector2Int _bufferOffset;
    public float cellSize = 1;

    public List<RoomSpace> Spaces {get; private set;}


    private List<RectInt> testRects = new List<RectInt>();

    void OnDrawGizmos()
    {
        if(grid == null)
        {
            return;
        }

        for (int i = 0; i < maxSize.x; i++)
        {
            for (int j = 0; j < maxSize.y; j++)
            {
                var pos = new Vector3(i, 0f, j);
                switch (grid[i, j]){
                    // case CellType.Buffer:
                    //     Gizmos.color = Color.white;
                    //     Gizmos.DrawSphere(pos, cellSize/6f);
                    //     break;
                    case CellType.Hallway:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(pos, cellSize/4f);
                        break;
                    // case CellType.WallEast:
                    // case CellType.WallWest:
                    // case CellType.WallSouth:
                    // case CellType.WallNorth:
                    //     Gizmos.color = Color.yellow;
                    //     Gizmos.DrawSphere(pos, cellSize/2f);
                    //     break;
                    case CellType.CornerOutNE:
                    case CellType.CornerOutNW:
                    case CellType.CornerOutSE:
                    case CellType.CornerOutSW:
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(pos, cellSize/1.5f);
                        break;
                    case CellType.CornerInNE:
                    case CellType.CornerInNW:
                    case CellType.CornerInSE:
                    case CellType.CornerInSW:
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(pos, cellSize/1.5f);
                        break;

                } 
            }
        }
    
        foreach(var rect in testRects)
        {
            var pos = new Vector3((rect.position + rect.size/2).x, 0f, (rect.position + rect.size/2).y);
            Gizmos.DrawWireCube(pos, new Vector3(rect.size.x, 0f, rect.size.y));
            Handles.Label(pos + Vector3.up*5, rect.size.ToString());
        }
    }

    public void BuildGrid(List<GameObject> objects)
    {
        grid = new PF_Grid<CellType>(maxSize, offset);
        Spaces = new List<RoomSpace>();

        foreach(var obj in objects)
        {
            PlaceRoom(obj);
        }
    }

    public void PlaceRoom(GameObject room)
    {
        var placed = false;
        
        while(!placed)
        {
            var obj = Instantiate(room, Vector3.zero, Quaternion.identity, this.transform);
            var pos = new Vector3(
                Random.Range(padding.x/2, maxSize.x-padding.x/2),
                0f,
                Random.Range(padding.y/2, maxSize.y-padding.y/2)
            );

            obj.transform.position = pos;
            RotateContent(obj);
            var objVar = obj.GetComponent<PF_Room>();
            var rect = new RectInt(objVar.Location2D, objVar.Size2D);

            if(CheckRoomInBounds(rect) && CheckInsersect(rect)){
                AddBuffer(rect);
                foreach(var x in rect.allPositionsWithin)
                {
                    if(objVar.GetType() == typeof(PF_Room)){
                        grid[x] = CellType.Room;
                    }
                    else if (objVar.GetType() == typeof(PF_Extra)){
                        grid[x] = CellType.Hallway;
                    }
                }

                var space = new RoomSpace(rect.position, rect.size);
                space.AddEntrances(objVar.entrancePositions);
                Spaces.Add(space);
                placed = true;
            }
            else{
                Destroy(obj);
            }
        }
    }

    private void AddBuffer(RectInt area)
    {
        var size  = area.size;
        var pos = area.position;

        testRects.Add(area);

        for(int i = 0; i < Mathf.Abs(size.y); i++)
        {
            for (int p = 1; p < buffer.x; p++)
            {
                var y = size.y >= 0 ? i:-i;

                var xPos = pos + new Vector2Int(p, y);
                var xPos2 = pos - new Vector2Int(p, -y);
                var xPos3 = pos + new Vector2Int(p + size.x, y);
                var xPos4 = pos - new Vector2Int(p - size.x, -y);

                grid[xPos] = CellType.Buffer;
                grid[xPos2] = CellType.Buffer;
                grid[xPos3] = CellType.Buffer;
                grid[xPos4] = CellType.Buffer;
            }
        }

        for(int i = 0; i < Mathf.Abs(size.x); i++)
        {
            for (int p = 1; p < buffer.y; p++)
            {
                var y = size.x >= 0 ? i:-i;

                var xPos = pos + new Vector2Int(y, p);
                var xPos2 = pos - new Vector2Int(-y, p);
                var xPos3 = pos + new Vector2Int(y, p + size.y);
                var xPos4 = pos - new Vector2Int(-y, p - size.y);

                grid[xPos] = CellType.Buffer;
                grid[xPos2] = CellType.Buffer;
                grid[xPos3] = CellType.Buffer;
                grid[xPos4] = CellType.Buffer;
            }
        }
    }

    private bool CheckRoomInBounds(RectInt area)
    {
        foreach(var x in area.allPositionsWithin)
        {
            if(!grid.InBounds(x))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckInsersect(RectInt area)
    {
        foreach (var pos in area.allPositionsWithin)
        {
            if(grid[pos] is CellType.Room or CellType.Buffer)
            {
                return false;
            }
        }

        return true;
    }

    private void RotateContent(GameObject obj)
    {
        int rotVal = Random.Range(0, 4);
        var rotQ = Quaternion.Euler(0f, 90*rotVal, 0f);

        obj.transform.rotation *= rotQ;

        var roomComp = obj.GetComponent<PF_Room>();
        var currentSize = roomComp.roomSize;
        var currentOffset = roomComp.roomSizeOffset;

        var newSize = rotQ * currentSize;
        currentOffset = newSize/2f;

        roomComp.roomSize = newSize;

        switch (rotVal)
        {
            case 0:
                roomComp.pivotCorner = roomComp.firstCorner;
                _bufferOffset = new Vector2Int(buffer.x, buffer.y);
                break;
            case 1:
                roomComp.pivotCorner = roomComp.firstCorner;
                currentOffset.z = 0;
                _bufferOffset = new Vector2Int(buffer.x, -buffer.y);
                break;
            case 2:
                roomComp.pivotCorner = roomComp.firstCorner;
                currentOffset.x = 0;
                _bufferOffset = new Vector2Int(-buffer.x, -buffer.y);
                break;
            case 3:
                roomComp.pivotCorner = roomComp.firstCorner;
                currentOffset.z = 0;
                _bufferOffset = new Vector2Int(-buffer.x, buffer.y);
                break;
        }
    }

    public void SetCoordinate(Vector2Int pos, CellType type)
    {
        grid[pos] = type;
    }

    public void MarkWalls()
    {
        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                grid[x, y] = grid[x, y] switch
                {
                    CellType.Hallway when grid[x - 1, y] is CellType.None or CellType.Buffer => CellType.WallWest,
                    CellType.Hallway when grid[x + 1, y] is CellType.None or CellType.Buffer => CellType.WallEast,
                    CellType.Hallway when grid[x, y - 1] is CellType.None or CellType.Buffer => CellType.WallSouth,
                    CellType.Hallway when grid[x, y + 1] is CellType.None or CellType.Buffer => CellType.WallNorth,
                    _ => grid[x, y]
                };
            }
        }

        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                switch (grid[x, y])
                {
                    case CellType.WallEast:
                        if (grid[x, y + 1] is CellType.None or CellType.Buffer && grid[x, y - 1] == CellType.WallEast &&
                            grid[x - 1, y] == CellType.Hallway)
                        {
                            grid[x, y] = CellType.CornerOutNE;
                        }

                        if (grid[x, y + 1] == CellType.WallEast && grid[x, y - 1] is CellType.None or CellType.Buffer &&
                            grid[x - 1, y] == CellType.Hallway)
                        {
                            grid[x, y] = CellType.CornerOutSE;
                        }

                        switch (grid[x - 1, y])
                        {
                            case CellType.WallNorth:
                                grid[x, y] = CellType.CornerOutNE;
                                break;
                            case CellType.WallSouth:
                                grid[x, y] = CellType.CornerOutSE;
                                break;
                        }
                        break;
                    case CellType.WallWest:
                        if (grid[x, y + 1] is CellType.None or CellType.Buffer && grid[x, y - 1] == CellType.WallWest &&
                            grid[x + 1, y] == CellType.Hallway)
                        {
                            grid[x, y] = CellType.CornerOutNW;
                            break;
                        }

                        if (grid[x, y + 1] == CellType.WallWest && grid[x, y - 1] is CellType.None or CellType.Buffer &&
                            grid[x + 1, y] == CellType.Hallway
                            || (grid[x, y + 1] == CellType.Hallway && grid[x - 1, y + 1] is CellType.WallWest or CellType.CornerOutSW))
                        {
                            grid[x, y] = CellType.CornerOutSW;
                            break;
                        }

                        switch (grid[x + 1, y])
                        {
                            case CellType.WallNorth:
                                grid[x, y] = CellType.CornerOutNW;
                                break;
                            case CellType.WallSouth:
                                grid[x, y] = CellType.CornerOutSW;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
