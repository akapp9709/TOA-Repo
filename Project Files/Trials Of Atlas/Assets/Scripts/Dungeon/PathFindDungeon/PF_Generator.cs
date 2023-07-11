/*
 * Adapted from https://github.com/vazgriz/DungeonGenerator
 *
 * Has been modified to make use of room prefabs instead of randomly sized cubes, and to have additional visual aids
 * through Gizmos
 * Documentation and algorithm details can be found here: https://vazgriz.com/119/procedurally-generated-dungeons/
 */

using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class PF_Generator : MonoBehaviour
{
    private class RoomSpace
    {
        public RectInt bounds;
        public List<Vector2Int> roomEntrances = new List<Vector2Int>();

        public RoomSpace(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public void AddEntrances(List<Transform> positions)
        {
            foreach (var t in positions)
            {
                TranslateAndAddPosition(t.position);
            }
        }

        public void TranslateAndAddPosition(Vector3 v)
        {
            var vec = new Vector2Int((int) v.x, (int) v.z);
            roomEntrances.Add(vec);
        }

        public Vector2Int GetClosestEntrance(Vector2Int pos)
        {
            var vec = roomEntrances[0];

            foreach (var v in roomEntrances)
            {
                if (Vector2Int.Distance(v, pos) <= Vector2Int.Distance(vec, pos))
                {
                    vec = v;
                }
            }

            return vec;
        }
    }
    public enum CellType
    {
        None,
        Buffer,
        Room,
        Hallway,
        WallEast,
        WallWest,
        WallSouth,
        WallNorth,
        CornerOutNE,
        CornerOutNW,
        CornerOutSE,
        CornerOutSW,
        CornerInNE,
        CornerInNW,
        CornerInSE,
        CornerInSW
    }
    
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject wallTile, cornerTile;
    private PF_Grid<CellType> _grid;
    public Vector2Int maxSize, offset, roomBuffer;
    public int dungeonLength;

    #region Debugging Variables
    
    private List<Vector3> _testPoints = new List<Vector3>();
    private List<Vector3> _hallPos = new List<Vector3>();
    private List<Vector3> _entrances = new List<Vector3>();
    private List<RectInt> _testRects = new List<RectInt>();
    private List<RectInt> _testBuffer = new List<RectInt>();
    private List<Vertex> _vertices;
    private List<PF_Delauney.Edge> _edges;

    #endregion
    
    private Vector2Int _rectOffset;
    private List<GameObject> _placedRooms = new List<GameObject>();
    private List<RoomSpace> _rooms = new List<RoomSpace>();
    private PF_Delauney _delaunay;
    private HashSet<Prim.Edge> _selectedEdges = new HashSet<Prim.Edge>();

    private List<MeshFilter> _floorMeshes = new List<MeshFilter>();
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // var offPos = new Vector3(offset.x, 0f, offset.y);
        // var gridSize = new Vector3(maxSize.x, 0f, maxSize.y);
        // Gizmos.DrawWireCube(gridSize/2, gridSize);
        //
        // foreach (var x in _testPoints)
        // {
        //     Gizmos.DrawWireSphere(x, 0.4f);
        // }
        //
        // Gizmos.color = Color.yellow;
        // foreach (var x in _testBuffer)
        // {
        //     Gizmos.DrawSphere(new Vector3(x.position.x, 0f, x.position.y), 0.5f);
        //     foreach (var y in x.allPositionsWithin)
        //     {
        //         var pos = new Vector3(y.x, 0f, y.y);
        //         Gizmos.DrawCube(pos, 0.4f*Vector3.one);
        //     }
        // }
        //
        // Gizmos.color = Color.blue;
        // foreach (var x in _testRects)
        // {
        //     Gizmos.DrawSphere(new Vector3(x.position.x, 0f, x.position.y), 0.5f);
        //     foreach (var y in x.allPositionsWithin)
        //     {
        //         var pos = new Vector3(y.x, 0f, y.y);
        //         Gizmos.DrawCube(pos, 0.4f*Vector3.one);
        //     }
        // }
        //
        // if (_rooms.Count > 0)
        // {
        //     foreach (var room in _rooms)
        //     {
        //         foreach (var pos in room.roomEntrances)
        //         {
        //             var vec = new Vector3(pos.x, 0f, pos.y) + Vector3.up;
        //             Gizmos.DrawSphere(vec, 1.2f);
        //         }
        //     }
        // }
        //
        // Gizmos.color = Color.red;
        // if (_hallPos.Count > 0)
        // {
        //     foreach (var v in _hallPos)
        //     {
        //         Gizmos.DrawCube(v, Vector3.one);
        //     }
        // }
        //
        // if (_delaunay != null && _delaunay.Edges.Count > 0 && _selectedEdges.Count > 0)
        // {
        //     Gizmos.color = Color.green;
        //     foreach (var x in _vertices)
        //     {
        //         Gizmos.DrawSphere(x.Position, 0.5f);
        //     }
        //
        //     foreach (var edge in _delaunay.Edges)
        //     {
        //         Gizmos.DrawLine(edge.U.Position, edge.V.Position);
        //     }
        //     
        //     Gizmos.color = Color.red;
        //     foreach (var edge in _selectedEdges)
        //     {
        //         Gizmos.DrawLine(edge.U.Position, edge.V.Position);
        //     }
        // }
        if (_grid == null)
            return;
        for (int i = 0; i < maxSize.y; i++)
        {
            for (int j = 0; j < maxSize.x; j++)
            {
                switch (_grid[j, i])
                {
                    case CellType.Hallway:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                    case CellType.WallEast:
                    case CellType.WallWest:
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                    case CellType.WallNorth:
                    case CellType.WallSouth:
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                    case CellType.CornerInNE:
                    case CellType.CornerInNW:
                    case CellType.CornerInSE:
                    case CellType.CornerInSW:
                    case CellType.CornerOutNE:
                    case CellType.CornerOutNW:
                    case CellType.CornerOutSE:
                    case CellType.CornerOutSW:
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                    case CellType.Room:
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                }
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _grid = new PF_Grid<CellType>(maxSize, offset);
        _rectOffset = new Vector2Int(1, 0);

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathFindHallways();
        MarkWalls();
        BuildHallways();
        CombineHallways();
    }

    #region RoomPlacement
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
            var rs = new RoomSpace(roomRect.position, roomRect.size);
            
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
                _placedRooms.Add(room);
                rs.AddEntrances(roomVar.entrancePositions);
                _rooms.Add(rs);
            }
            else
            {
                Destroy(room);
                i--;
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
    
    #endregion

    #region Triangulation
    private void Triangulate()
    {
        _vertices = new List<Vertex>();
        _edges = new List<PF_Delauney.Edge>();
        foreach (var room in _rooms)
        {
            _vertices.Add(new Vertex<RoomSpace>((Vector2)room.bounds.position + ((Vector2) room.bounds.size / 2), room));
        }

        _delaunay = PF_Delauney.Triangulate(_vertices);
        _edges = _delaunay.Edges;
    }
    #endregion

    #region Hallways

    private void CreateHallways()
    {
        List<Prim.Edge> pEdges = new List<Prim.Edge>();

        foreach (var edge in _delaunay.Edges)
        {
            pEdges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(pEdges, pEdges[0].U);

        _selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(pEdges);
        remainingEdges.ExceptWith(_selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (Random.Range(0, 1f) < 0.2f)
            {
                _selectedEdges.Add(edge);
            }
        }
    }
    #endregion

    #region Pathfinding Hallways

    private void PathFindHallways()
    {
        var aStar = new PF_Pathfinder(maxSize);

        foreach (var edge in _selectedEdges)
        {
            var startRoom = (edge.U as Vertex<RoomSpace>).Item;
            var endRoom = (edge.V as Vertex<RoomSpace>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;

            var startPos = startRoom.GetClosestEntrance(Vector2Int.FloorToInt(startPosf));
            var endPos = endRoom.GetClosestEntrance(startPos);

            var path = aStar.FindPath(startPos, endPos, (PF_Pathfinder.Node a, PF_Pathfinder.Node b) =>
            {
                var cost = new PF_Pathfinder.PathCost();

                switch (_grid[b.Position])
                {
                    case CellType.None:
                        cost.cost += 5;
                        break;
                    case CellType.Buffer:
                        cost.cost += 6;
                        break;
                    case CellType.Hallway:
                        cost.cost += 1;
                        break;
                    case CellType.Room:
                        cost.cost += 20;
                        break;
                    default:
                        break;
                }

                cost.traversable = true;

                return cost;
            });
            
            if(path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];
                    if (_grid[current] == CellType.None)
                    {
                        _grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];
                        var delta = current - prev;
                    }
                }
                
                foreach (var pos in path)
                {
                    foreach (var offset in hallOffset)
                    {
                        PlaceHallway(pos);
                        var x = _grid[pos + offset];
                        if (x == CellType.Buffer || x == CellType.None)
                        {
                            _grid[pos + offset] = CellType.Hallway;
                            PlaceHallway(pos + offset);
                        }
                    }
                }
            }
        }
    }

    private void PlaceHallway(Vector2Int pos)
    {
        var vec = new Vector3(pos.x, 0f, pos.y);
        if (!_hallPos.Contains(vec))
        {
            _hallPos.Add(vec);
        }
        // var tile = Instantiate(floorTile, vec, Quaternion.identity);
        // _floorMeshes.Add(tile.GetComponent<MeshFilter>());
    }

    private Vector2Int[] hallOffset =
    {
        new Vector2Int(-1,1),
        new Vector2Int(-1,0),
        new Vector2Int(-1, -1),
        new Vector2Int(0,1),
        new Vector2Int(0,0),
        new Vector2Int(0, -1),
        new Vector2Int(1,1),
        new Vector2Int(1,0),
        new Vector2Int(1, -1)
    };

    private void CombineHallways()
    {
        var combine = new CombineInstance[_floorMeshes.Count];

        for (int i = 0; i < _floorMeshes.Count; i++)
        {
            combine[i].mesh = _floorMeshes[i].mesh;
            combine[i].transform = _floorMeshes[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = mesh;

        foreach (var obj in _floorMeshes)
        {
            Destroy(obj.gameObject);
        }
    }

    private void MarkWalls()
    {
        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                _grid[x, y] = _grid[x, y] switch
                {
                    CellType.Hallway when _grid[x - 1, y] == CellType.None => CellType.WallWest,
                    CellType.Hallway when _grid[x + 1, y] == CellType.None => CellType.WallEast,
                    CellType.Hallway when _grid[x, y - 1] == CellType.None => CellType.WallSouth,
                    CellType.Hallway when _grid[x, y + 1] == CellType.None => CellType.WallNorth,
                    _ => _grid[x, y]
                };
            }
        }
        
        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                switch (_grid[x, y])
                {
                    case CellType.WallEast:
                        switch (_grid[x - 1, y])
                        {
                            case CellType.WallNorth:
                                _grid[x, y] = CellType.CornerOutNE;
                                break;
                            case CellType.WallSouth:
                                _grid[x, y] = CellType.CornerOutSE;
                                break;
                        }
                        break;
                    case CellType.WallWest:
                        switch (_grid[x + 1, y])
                        {
                            case CellType.WallNorth:
                                _grid[x, y] = CellType.CornerOutNW;
                                break;
                            case CellType.WallSouth:
                                _grid[x, y] = CellType.CornerOutSW;
                                break;
                        }
                        break;
                }
            }
        }
    }

    private void BuildHallways()
    {
        for (int i = 0; i < maxSize.y; i++)
        {
            for (int j = 0; j < maxSize.x; j++)
            {
                switch (_grid[j, i])
                {
                    case CellType.Hallway:
                        PlaceFloor(new Vector2Int(j, i));
                        break;
                    case CellType.WallEast:
                    case CellType.WallNorth:
                    case CellType.WallSouth:
                    case CellType.WallWest:
                        PlaceWall(new Vector2Int(j,i),_grid[j, i]);
                        break;
                    case CellType.CornerOutNE:
                    case CellType.CornerOutNW:
                    case CellType.CornerOutSE:
                    case CellType.CornerOutSW:
                        PlaceCorner(new Vector2Int(j, i), _grid[j, i]);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PlaceFloor(Vector2Int pos)
    {
        var vec = new Vector3(pos.x, 0f, pos.y);
        
        var tile = Instantiate(floorTile, vec, Quaternion.identity);
        _floorMeshes.Add(tile.GetComponent<MeshFilter>());
    }

    private void PlaceWall(Vector2Int pos, CellType type)
    {
        var vec = new Vector3(pos.x, 0f, pos.y);
        var wall = Instantiate(wallTile, vec, Quaternion.identity);
        _floorMeshes.Add(wall.GetComponent<MeshFilter>());
        switch (type)
        {
            case CellType.WallWest:
                break;
            case CellType.WallNorth:
                wall.transform.Rotate(Vector3.up, 90f);
                break;
            case CellType.WallEast:
                wall.transform.Rotate(Vector3.up, 180f);
                break;
            case CellType.WallSouth:
                wall.transform.Rotate(Vector3.up, 270f);
                break;
            default:
                break;
        }
    }

    private void PlaceCorner(Vector2Int pos, CellType type)
    {
        var vec = new Vector3(pos.x, 0f, pos.y);
        var corner = Instantiate(cornerTile, vec, Quaternion.identity);
        _floorMeshes.Add(corner.GetComponent<MeshFilter>());
        switch (type)
        {
            case CellType.CornerOutNW:
                break;
            case CellType.CornerOutNE:
                corner.transform.Rotate(Vector3.up, 90f);
                break;
            case CellType.CornerOutSE:
                corner.transform.Rotate(Vector3.up, 180f);
                break;
            case CellType.CornerOutSW:
                corner.transform.Rotate(Vector3.up, 270f);
                break;
            default:
                break;
        }
    }

    #endregion
}
