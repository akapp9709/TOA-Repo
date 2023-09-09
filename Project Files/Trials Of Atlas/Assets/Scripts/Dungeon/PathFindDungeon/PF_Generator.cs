/*
 * Adapted from https://github.com/vazgriz/DungeonGenerator
 *
 * Has been modified to make use of room prefabs instead of randomly sized cubes, and to have additional visual aids
 * through Gizmos
 * Documentation and algorithm details can be found here: https://vazgriz.com/119/procedurally-generated-dungeons/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Graphs;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class PF_Generator : MonoBehaviour
{
    [Serializable]
    private class RoomVariant
    {
        public GameObject room;
        public int formationIndex;
        public float roomValue, roomBias, shiftedBias;
    }
    
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
        CornerInSW,
        SoftBuffer,
        DiagonalEastDown,
        DiagonalWestDown,
        DiagonalEastUp,
        DiagonalWestUp
    }

    [SerializeField] private PlayerSO playerStats;
    [SerializeField] private GameObject startRoom, bossRoom;
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject wallTile, cornerTile, diagonalTile;
    private PF_Grid<CellType> _grid;
    public Vector2Int maxSize, padding, offset, roomBuffer;
    public int dungeonLength, availableRooms = 0;

    #region Debugging Variables
    
    private List<RectInt> _testRects = new List<RectInt>();
    private List<RectInt> _testBuffer = new List<RectInt>();
    private List<Vertex> _vertices;
    private List<PF_Delauney.Edge> _edges;
    [SerializeField]private List<RoomVariant> placedVars;
    
    #endregion
    
    private Vector2Int _rectOffset;
    private List<GameObject> _placedRooms = new List<GameObject>();
    private List<RoomSpace> _rooms = new List<RoomSpace>();
    private PF_Delauney _delaunay;
    private HashSet<Prim.Edge> _selectedEdges = new HashSet<Prim.Edge>();

    private List<MeshFilter> _floorMeshes = new List<MeshFilter>();

    private int _roomRotation;
    public bool showGizmos = true;
    public bool showTriangulation = true;
    public bool showPath = true;

    [SerializeField] private List<RoomVariant> variants = new List<RoomVariant>();
    public float minRoomValue, maxRoomValue, roomCount;
    [Range(1, 5)] public float curveWidth;
    [Range(0, 1)] public float curveOffset;
    private float _lineLength;

    private void OnDrawGizmos()
    {
        if (_delaunay != null && _delaunay.Vertices != null && showTriangulation)
        {
            Gizmos.color = Color.green;
            foreach (var pnt in _delaunay.Vertices)
            {
                Gizmos.DrawSphere(new Vector3(pnt.Position.x, 0f, pnt.Position.y) + Vector3.up * 6, 1);
            }

            foreach (var edge in _delaunay.Edges)
            {
                var posU = new Vector3(edge.U.Position.x, 0f, edge.U.Position.y) + Vector3.up * 6;
                var posV = new Vector3(edge.V.Position.x, 0f, edge.V.Position.y) + Vector3.up * 6;
                Gizmos.DrawLine(posU, posV);
            }
        }

        if (_selectedEdges.Count > 0 && showPath)
        {
            Gizmos.color = Color.red;
            foreach (var edge in _selectedEdges)
            {
                var posU = new Vector3(edge.U.Position.x, 0f, edge.U.Position.y) + Vector3.up * 6;
                var posV = new Vector3(edge.V.Position.x, 0f, edge.V.Position.y) + Vector3.up * 6;
                Gizmos.DrawLine(posU, posV);
            }
        }
        
        if (!showGizmos)
            return;
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
                    case CellType.SoftBuffer:
                        Gizmos.color = Color.white;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.3f);
                        break;
                    case CellType.DiagonalEastDown:
                    case CellType.DiagonalWestDown:
                    case CellType.DiagonalEastUp:
                    case CellType.DiagonalWestUp:
                        Gizmos.color = Color.black;
                        Gizmos.DrawSphere(new Vector3(j, 0f, i) + Vector3.up, 0.5f);
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

        PrepareValues();
        PlaceRooms();
        // Triangulate();
        // CreateHallways();
        // //StartCoroutine(PathFindHallways());
        // PathFindHallways();
        // MarkWalls();
        // BuildHallways();
        // CombineHallways();
    }

    #region Value Prep

    private void PrepareValues()
    {
        foreach (var room in roomPrefabs)
        {
            var roomObj = room.GetComponent<PF_Room>();
            var length = roomObj.Variants.Count;
            
            for (int i = 0; i < length; i++)
            {
                var obj = new RoomVariant();
                obj.room = room;
                obj.formationIndex = i;
                roomObj.Variants[i].CalculateRoomValue();
                obj.roomValue = roomObj.Variants[i].formationValue;
                variants.Add(obj);
            }
        }
        
        SortRooms();
        minRoomValue = variants[0].roomValue;
        maxRoomValue = variants[^1].roomValue;
        roomCount = variants.Count;

        CalculateBiases();
        PrepareVariantLine();

        if (dungeonLength > availableRooms)
        {
            dungeonLength = availableRooms;
        }

        _lineLength = variants[^1].shiftedBias;
    }

    private void SortRooms()
    {
        for (int i = 0; i < variants.Count-1; i++)
        {
            for (int j = i+1; j < variants.Count; j++)
            {
                var x = variants[i].roomValue;
                var y = variants[j].roomValue;

                if (x > y)
                {
                    (variants[i], variants[j]) = (variants[j], variants[i]);
                }
            }
        }
    }

    private void CalculateBiases()
    {
        var runNum = playerStats.runCompleted;
        
        foreach (var room in variants)
        {
            room.roomBias =
                Mathf.Exp(-(Mathf.Pow(((room.roomValue / maxRoomValue) - (runNum / roomCount) - curveOffset), 2) /
                            (2 * Mathf.Pow(curveWidth, 2) / 100) ) );

            if (room.roomBias < 0.01)
            {
                room.roomBias = 0;
            }
        }
    }
    
    private void PrepareVariantLine()
    {
        variants[0].shiftedBias = variants[0].roomBias;
        for (int i = 1; i < variants.Count; i++)
        {
            variants[i].shiftedBias += variants[i - 1].shiftedBias + variants[i].roomBias;
            if (variants[i].roomBias != 0)
            {
                availableRooms++;
            }
        }
    }
    #endregion

    #region RoomPlacement
    private void PlaceRooms() 
    {
        PlaceStartRoom();
        PlaceBossRoom();

        // PlaceDungeonRooms();
        StartCoroutine(PlaceDungeonRoomsRoutine());
    }

    private void PlaceDungeonRooms()
    {
        for (int i = 0; i < dungeonLength; i++)
        {
            var pos = new Vector3(
                Random.Range((int) (0.5f * padding.x), (int) ((maxSize.x + 1) - 0.5f * padding.x)),
                0f,
                Random.Range((int) (0.5f * padding.y), (int) ((maxSize.x + 1) - 0.5f * padding.y))
            );

            var x = Random.Range(0f, _lineLength);
            int ind = 0;

            for (int j = 0; j < variants.Count; j++)
            {
                if (x > variants[j].shiftedBias)
                {
                    continue;
                }

                ind = j;
                break;
            }

            placedVars.Add(variants[ind]);
            var room = Instantiate(variants[ind].room, pos, Quaternion.identity, this.transform);
            ChooseRoomRotation(room, out var rot);

            var roomVar = room.GetComponent<PF_Room>();

            //TranslateBounds(rot, out var rectPosOff, out var rectSizeOff);
            var roomRect = new RectInt(roomVar.Location2D, roomVar.Size2D);

            var rs = new RoomSpace(roomRect.position, roomRect.size);

            var roomBufferRect = new RectInt(roomRect.position - _rectOffset, roomRect.size + _rectOffset * 2);

            bool safe = true;
            var inBox = CheckRoomInBounds(roomBufferRect);

            if (!inBox)
            {
                safe = false;
            }
            else if (!CheckRoomIntersect(roomBufferRect))
            {
                safe = false;
            }

            if (safe)
            {
                _testBuffer.Add(roomBufferRect);
                _testRects.Add(roomRect);
                roomVar.PlaceEnemies(variants[ind].formationIndex);
                FinalizeRoom(roomRect, rot);
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

        foreach (var obj in _placedRooms)
        {
            obj.transform.position -= new Vector3(0.5f, 0f, 0.5f);
        }
    }
    
    private IEnumerator PlaceDungeonRoomsRoutine()
    {
        for (int i = 0; i < dungeonLength; i++)
        {
            var pos = new Vector3(
                Random.Range((int) (0.5f * padding.x), (int) ((maxSize.x + 1) - 0.5f * padding.x)),
                0f,
                Random.Range((int) (0.5f * padding.y), (int) ((maxSize.x + 1) - 0.5f * padding.y))
            );

            var x = Random.Range(0f, _lineLength);
            int ind = 0;

            for (int j = 0; j < variants.Count; j++)
            {
                if (x > variants[j].shiftedBias)
                {
                    continue;
                }

                ind = j;
                break;
            }

            placedVars.Add(variants[ind]);
            var room = Instantiate(variants[ind].room, pos, Quaternion.identity, this.transform);
            ChooseRoomRotation(room, out var rot);

            var roomVar = room.GetComponent<PF_Room>();

            //TranslateBounds(rot, out var rectPosOff, out var rectSizeOff);
            var roomRect = new RectInt(roomVar.Location2D, roomVar.Size2D);

            var rs = new RoomSpace(roomRect.position, roomRect.size);

            var roomBufferRect = new RectInt(roomRect.position - _rectOffset, roomRect.size + _rectOffset * 2);

            bool safe = true;
            var inBox = CheckRoomInBounds(roomBufferRect);

            if (!inBox)
            {
                safe = false;
            }
            else if (!CheckRoomIntersect(roomBufferRect))
            {
                safe = false;
            }

            if (safe)
            {
                _testBuffer.Add(roomBufferRect);
                _testRects.Add(roomRect);
                roomVar.PlaceEnemies(variants[ind].formationIndex);
                FinalizeRoom(roomRect, rot);
                _placedRooms.Add(room);
                rs.AddEntrances(roomVar.entrancePositions);
                _rooms.Add(rs);

                GetComponent<NavMeshSurface>().navMeshData = null;
                GetComponent<NavMeshSurface>().BuildNavMesh();

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                Destroy(room);
                i--;
                
                GetComponent<NavMeshSurface>().navMeshData = null;
                GetComponent<NavMeshSurface>().BuildNavMesh();
                
                yield return new WaitForSeconds(0.2f);
            }
        }

        foreach (var obj in _placedRooms)
        {
            obj.transform.position -= new Vector3(0.5f, 0f, 0.5f);
            GetComponent<NavMeshSurface>().navMeshData = null;
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        
        Triangulate();
        CreateHallways();
        //StartCoroutine(PathFindHallways());
        PathFindHallways();
        MarkWalls();
        BuildHallways();
        CombineHallways();
    }

    private void PlaceBossRoom()
    {
        var bossPlaced = false;
        while (!bossPlaced)
        {
            var bossPos = new Vector3(
                Random.Range((int) (0.5f * padding.x), (int) ((maxSize.x + 1) - 0.5f * padding.x)),
                0f,
                Random.Range((int) (0.5f * padding.y), (int) ((maxSize.x + 1) - 0.5f * padding.y))
            );

            var boss = Instantiate(this.bossRoom, bossPos, Quaternion.identity);
            ChooseRoomRotation(boss, out var bRot);
            var bossVar = boss.GetComponent<PF_Room>();
            var bossRect = new RectInt(bossVar.Location2D, bossVar.Size2D);
            var bossSpace = new RoomSpace(bossRect.position, bossRect.size);
            var bossBuffer = new RectInt(bossRect.position - _rectOffset, bossRect.size + _rectOffset * 2);

            if (CheckRoomInBounds(bossBuffer) && CheckRoomIntersect(bossBuffer))
            {
                _testBuffer.Add(bossBuffer);
                _testRects.Add(bossRect);
                FinalizeRoom(bossRect, bRot);
                _placedRooms.Add(boss);
                bossSpace.AddEntrances(bossVar.entrancePositions);
                _rooms.Add(bossSpace);
                bossPlaced = true;
            }
            else
            {
                Destroy(boss);
            }
        }
    }

    private void PlaceStartRoom()
    {
        var startPlaced = false;
        while (!startPlaced)
        {
            var startPos = new Vector3(
                Random.Range((int) (0.5f * padding.x), (int) ((maxSize.x + 1) - 0.5f * padding.x)),
                0f,
                Random.Range((int) (0.5f * padding.y), (int) ((maxSize.x + 1) - 0.5f * padding.y))
            );

            var start = Instantiate(this.startRoom, startPos, Quaternion.identity);
            ChooseRoomRotation(start, out var bRot);
            var startVar = start.GetComponent<PF_Room>();
            var startRect = new RectInt(startVar.Location2D, startVar.Size2D);
            var startSpace = new RoomSpace(startRect.position, startRect.size);
            var startBuffer = new RectInt(startRect.position - _rectOffset, startRect.size + _rectOffset * 2);

            if (CheckRoomInBounds(startBuffer) && CheckRoomIntersect(startBuffer))
            {
                _testBuffer.Add(startBuffer);
                _testRects.Add(startRect);
                FinalizeRoom(startRect, bRot);
                _placedRooms.Add(start);
                startSpace.AddEntrances(startVar.entrancePositions);
                _rooms.Add(startSpace);
                startPlaced = true;
            }
            else
            {
                Destroy(start);
            }
        }
    }

    private void ChooseRoomRotation(GameObject room, out int rotation)
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

        rotation = rotVal;
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

    private void FinalizeRoom(RectInt area, int val)
    {
        AddSoftBuffer(val, area);
        foreach (var pos in area.allPositionsWithin)
        {
            _grid[pos] = CellType.Room;
        }
    }

    private void AddSoftBuffer(int val, RectInt area)
    {
        var vec = new Vector2Int();
        var size = new Vector2Int();
        switch (val)
        {
            case 1:
                vec = area.position + new Vector2Int(-1, -1);
                size = area.size + new Vector2Int(2,2);
                break;
            case 2:
                vec = area.position + new Vector2Int(-1, -1);
                size = area.size + new Vector2Int(2,2);
                break;
            case 3:
                vec = area.position + new Vector2Int(-1, 1);
                size = area.size + new Vector2Int(2,-2);
                break;
            case 4:
                vec = area.position + new Vector2Int(1, -1);
                size = area.size + new Vector2Int(-2,2);
                break;
        }
        var rect = new RectInt(vec, size);
        foreach (var pos in rect.allPositionsWithin)
        {
            _grid[pos] = CellType.SoftBuffer;
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
            if (Random.Range(0, 1f) < 0.3f)
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

                cost.cost += Vector2Int.Distance(b.Position, maxSize/2) * 0.2f;

                switch (_grid[b.Position])
                {
                    case CellType.None:
                        cost.cost += 9;
                        break;
                    case CellType.Buffer:
                        cost.cost += 10;
                        break;
                    case CellType.Hallway:
                        cost.cost += 1;
                        break;
                    case CellType.SoftBuffer:
                        cost.cost += 30;
                        break;
                    case CellType.Room:
                        cost.cost += 100;
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
                        var x = _grid[pos + offset];
                        if (x != CellType.Room)
                            _grid[pos + offset] = CellType.Hallway;
                    }
                }
            }
        }
        //
        // MarkWalls();
        // BuildHallways();
        // CombineHallways();
    }

    private Vector2Int[] hallOffset =
    {
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,0),
        new Vector2Int(0, -1),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(1,0),
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
        GetComponent<MeshCollider>().sharedMesh = mesh;

        foreach (var obj in _floorMeshes)
        {
            Destroy(obj.gameObject);
        }

        showGizmos = false;
    }

    private void MarkWalls()
    {
        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                _grid[x, y] = _grid[x, y] switch
                {
                    CellType.Hallway when _grid[x - 1, y] is CellType.None or CellType.SoftBuffer => CellType.WallWest,
                    CellType.Hallway when _grid[x + 1, y] is CellType.None or CellType.SoftBuffer => CellType.WallEast,
                    CellType.Hallway when _grid[x, y - 1] is CellType.None or CellType.SoftBuffer => CellType.WallSouth,
                    CellType.Hallway when _grid[x, y + 1] is CellType.None or CellType.SoftBuffer => CellType.WallNorth,
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
                        if (_grid[x, y + 1] is CellType.None or CellType.SoftBuffer && _grid[x, y - 1] == CellType.WallEast &&
                            _grid[x - 1, y] == CellType.Hallway)
                        {
                            _grid[x, y] = CellType.CornerOutNE;
                        }

                        if (_grid[x, y + 1] == CellType.WallEast && _grid[x, y - 1] is CellType.None or CellType.SoftBuffer &&
                            _grid[x - 1, y] == CellType.Hallway)
                        {
                            _grid[x, y] = CellType.CornerOutSE;
                        }
                        
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
                        if (_grid[x, y + 1] is CellType.None or CellType.SoftBuffer && _grid[x, y - 1] == CellType.WallWest &&
                            _grid[x + 1, y] == CellType.Hallway)
                        {
                            _grid[x, y] = CellType.CornerOutNW;
                            break;
                        }

                        if (_grid[x, y + 1] == CellType.WallWest && _grid[x, y - 1] is CellType.None or CellType.SoftBuffer &&
                            _grid[x + 1, y] == CellType.Hallway)
                        {
                            _grid[x, y] = CellType.CornerOutSW;
                            break;
                        }
                        
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
        
        for (int y = 0; y < maxSize.y; y++)
        {
            for (int x = 0; x < maxSize.x; x++)
            {
                switch (_grid[x, y])
                {
                    case CellType.WallEast:
                        if (_grid[x - 1, y + 1] is CellType.WallEast or CellType.DiagonalEastDown or CellType.CornerOutNE or CellType.WallNorth && 
                            _grid[x + 1, y - 1] is CellType.WallEast or CellType.DiagonalEastDown or CellType.CornerOutNE or CellType.WallNorth)
                        {
                            _grid[x, y] = CellType.DiagonalEastDown;
                            break;
                        }
                        if (_grid[x - 1, y - 1] is CellType.WallEast or CellType.DiagonalEastUp or CellType.CornerOutSE or CellType.WallSouth &&  
                            _grid[x + 1, y + 1] is CellType.WallEast or CellType.DiagonalEastUp or CellType.CornerOutSE or CellType.WallSouth)
                        {
                            _grid[x, y] = CellType.DiagonalEastUp;
                            break;
                        }
                        break;
                    case CellType.WallWest:
                        if (_grid[x - 1, y + 1] is CellType.WallWest or CellType.DiagonalWestDown or CellType.CornerOutSW or CellType.WallSouth && 
                            _grid[x + 1, y - 1] is CellType.WallWest or CellType.DiagonalWestDown or CellType.CornerOutSW or CellType.WallSouth)
                        {
                            _grid[x, y] = CellType.DiagonalWestDown;
                            break;
                        }
                        if (_grid[x - 1, y - 1] is CellType.WallWest or CellType.DiagonalWestUp or CellType.CornerOutNW or CellType.WallNorth && 
                            _grid[x + 1, y + 1] is CellType.WallWest or CellType.DiagonalWestUp or CellType.CornerOutNW or CellType.WallNorth)
                        {
                            _grid[x, y] = CellType.DiagonalWestUp;
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
                    case CellType.DiagonalEastDown:
                    case CellType.DiagonalEastUp:
                    case CellType.DiagonalWestDown:
                    case CellType.DiagonalWestUp:
                        PlaceDiagonal(new Vector2Int(j, i), _grid[j, i]);
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
    private void PlaceDiagonal(Vector2Int pos, CellType type)
    {
        var vec = new Vector3(pos.x, 0f, pos.y);
        var diagonal = Instantiate(diagonalTile, vec, Quaternion.identity);
        _floorMeshes.Add(diagonal.GetComponent<MeshFilter>());
        switch (type)
        {
            case CellType.DiagonalWestUp:
                break;
            case CellType.DiagonalEastDown:
                diagonal.transform.Rotate(Vector3.up, 90f);
                break;
            case CellType.DiagonalEastUp:
                diagonal.transform.Rotate(Vector3.up, 180f);
                break;
            case CellType.DiagonalWestDown:
                diagonal.transform.Rotate(Vector3.up, 270f);
                break;
            default:
                break;
        }
    }
    #endregion
}
