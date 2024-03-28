using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.AI.Navigation;

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private PlayerSO playerStats;
    public List<GameObject> content = new List<GameObject>();
    private List<RoomSpace> _placed = new List<RoomSpace>();
    private List<PF_Delauney.Edge> _edges;
    private HashSet<Prim.Edge> _pEdges = new HashSet<Prim.Edge>();
    private GridBuilder _grid;
    public AssetPack themePack;
    [SerializeField] private GameObject wall, floor, ceiling;
    public Action OnComplete;
    public int numberOfRooms => _placed.Count;
    private List<RoomVariant> roomVariants;

    private int initialLength;
    void OnDrawGizmos()
    {
        if (_edges == null)
            return;

        foreach (var line in _edges)
        {
            var pos1 = line.U.Position + Vector3.up * 10;
            var pos2 = line.V.Position + Vector3.up * 10;

            Gizmos.DrawLine(pos1, pos2);
            Gizmos.DrawCube(pos1, Vector3.one);
        }
    }

    private void Awake()
    {
        floor.GetComponent<MeshRenderer>().material = themePack.floorMat;
        wall.GetComponent<MeshRenderer>().material = themePack.wallMat;
        initialLength = content.Count;

        GetContent();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateAbstraction();
        FinishBuild();
    }

    private void FinishBuild()
    {
        CreateGuideStructure();
        TrimStructure();
        BuildConnectors();
        PlaceTiles();

        GenerateLights();

        OnComplete?.Invoke();

        Invoke("PopulateRooms", 0.2f);
        Invoke("BuildNav", 0.2f);
    }

    private void GetContent()
    {
        var holders = GetComponents<ContentManager>();
        foreach (var comp in holders)
        {
            content.AddRange(comp.ChooseContent());
            // if(comp.GetType() == typeof(RoomManager))
            // {
            //     var thing = (RoomManager)comp;
            //     roomVariants.AddRange(thing.variants);
            // }
        }
    }

    private void CreateAbstraction()
    {
        _grid = GetComponent<GridBuilder>();
        _grid.BuildGrid(content);
        _placed.AddRange(_grid.Spaces);
    }

    private void CreateGuideStructure()
    {
        var triBuilder = GetComponent<Triangulator>();
        triBuilder.BuildTriangulation(_placed);
        _edges = triBuilder.Edges;
    }

    private void TrimStructure()
    {
        List<Prim.Edge> pEdges = new List<Prim.Edge>();

        foreach (var edges in _edges)
        {
            pEdges.Add(new Prim.Edge(edges.U, edges.V));
        }
        Debug.Log(pEdges.Count);
        List<Prim.Edge> mst = Prim.MinimumSpanningTree(pEdges, pEdges[0].U);

        _pEdges = new HashSet<Prim.Edge>(mst);
        var remaining = new HashSet<Prim.Edge>(pEdges);
        remaining.ExceptWith(_pEdges);

        foreach (var edge in remaining)
        {
            if (Random.Range(0, 1f) < 0.3f)
                _pEdges.Add(edge);
        }
    }

    private void BuildConnectors()
    {
        var connecter = GetComponent<ConnecterBuilder>();
        connecter.BuildConnections(_grid, _pEdges);
        _grid.MarkWalls();
    }

    private void PlaceTiles()
    {
        var meshesFloor = new List<MeshFilter>();
        var meshesWall = new List<MeshFilter>();
        for (int i = 0; i < _grid.maxSize.x; i++)
        {
            for (int j = 0; j < _grid.maxSize.y; j++)
            {
                GameObject floorObj = null, wallObj = null;
                var pos = new Vector3(i, 0f, j);
                switch (_grid.grid[i, j])
                {
                    case GridBuilder.CellType.Hallway:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        break;
                    case GridBuilder.CellType.WallNorth:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 90f, 0f));
                        break;
                    case GridBuilder.CellType.WallSouth:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 270f, 0f));
                        break;
                    case GridBuilder.CellType.WallEast:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 180f, 0f));
                        break;
                    case GridBuilder.CellType.WallWest:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.wall, pos, Quaternion.identity);
                        break;
                    case GridBuilder.CellType.CornerOutSW:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 270f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutNE:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 90f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutSE:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 180f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutNW:
                        floorObj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        wallObj = Instantiate(themePack.corner, pos, Quaternion.identity);
                        break;
                }

                if (floorObj != null)
                {
                    meshesFloor.Add(floorObj.GetComponent<MeshFilter>());
                }
                if (wallObj != null)
                {
                    meshesWall.Add(wallObj.GetComponent<MeshFilter>());
                }
            }
        }

        CreateMesh(meshesFloor, floor);
        CreateMesh(meshesWall, wall, true);
    }

    void CreateMesh(List<MeshFilter> meshes, GameObject target, bool reverseFaces = false)
    {
        var combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].mesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        if(reverseFaces)
        {
            Array.Reverse(mesh.vertices);
            Array.Reverse(mesh.triangles);
        }
        target.GetComponent<MeshFilter>().mesh = mesh;
        target.GetComponent<MeshCollider>().sharedMesh = mesh;

        foreach (var obj in meshes)
        {
            Destroy(obj.gameObject);
        }
    }

    void GenerateLights()
    {
        if (TryGetComponent<LightManager>(out LightManager manager))
        {
            Debug.Log("Generating Lights");
            manager.PlaceLights(_grid.grid);
        }
    }

    private void PopulateRooms()
    {
        var manager = GetComponent<RoomManager>();
        var indices = manager.indices;
        var rooms = GetComponentsInChildren<PF_Room>();
        var numExtras = GetComponentsInChildren<PF_Extra>().Length;


        Debug.Log(rooms.Length + "        " + numExtras);
        for (int i = 0; i < rooms.Length - numExtras; i++)
        {
            rooms[i].transform.position -= new Vector3(0.5f, 0f, 0.5f);
            if (i > 0 && i > initialLength - 1)
            {
                var val = indices[i - initialLength];
                rooms[i].PlaceEnemies(manager.variants[val].formationIndex);
            }
        }
    }

    void BuildNav()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
