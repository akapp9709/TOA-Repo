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
    public Action OnComplete;
    public int numberOfRooms => _placed.Count;
    private List<RoomVariant> roomVariants;

    private int initialLength;
    void OnDrawGizmos()
    {
        if(_edges == null)
            return;

        foreach(var line in _edges)
        {
            var pos1 = line.U.Position + Vector3.up * 10;
            var pos2 = line.V.Position + Vector3.up * 10;

            Gizmos.DrawLine(pos1, pos2);
            Gizmos.DrawCube(pos1, Vector3.one);
        }
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().material = themePack.material;
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
        OnComplete?.Invoke();

        GenerateLights();
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

        foreach(var edge in remaining)
        {
            if(Random.Range(0, 1f) < 0.3f)
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
        var meshes = new List<MeshFilter>();
        for (int i = 0; i < _grid.maxSize.x; i++)
        {
            for (int j = 0; j < _grid.maxSize.y; j++)
            {
                GameObject obj = null;
                var pos = new Vector3(i, 0f, j);
                switch (_grid.grid[i, j])
                {
                    case GridBuilder.CellType.Hallway:
                        obj = Instantiate(themePack.floor, pos, Quaternion.identity);
                        break;
                    case GridBuilder.CellType.WallNorth:
                        obj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 90f, 0f));
                        break;
                    case GridBuilder.CellType.WallSouth:
                        obj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 270f, 0f));
                        break;
                    case GridBuilder.CellType.WallEast:
                        obj = Instantiate(themePack.wall, pos, Quaternion.Euler(0f, 180f, 0f));
                        break;
                    case GridBuilder.CellType.WallWest:
                        obj = Instantiate(themePack.wall, pos, Quaternion.identity);
                        break;
                    case GridBuilder.CellType.CornerOutSW:
                        obj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 270f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutNE:
                        obj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 90f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutSE:
                        obj = Instantiate(themePack.corner, pos, Quaternion.Euler(0f, 180f, 0f));
                        break;
                    case GridBuilder.CellType.CornerOutNW:
                        obj = Instantiate(themePack.corner, pos, Quaternion.identity);
                        break;
                }

                if(obj != null)
                    meshes.Add(obj.GetComponent<MeshFilter>());
            }
        }

        var combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].mesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        foreach (var obj in meshes)
        {
            Destroy(obj.gameObject);
        }
    }

    void GenerateLights()
    {
        if(TryGetComponent<LightManager>(out LightManager manager))
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
        for(int i = initialLength; i < rooms.Length-numExtras; i++)
        {
            var val = indices[i-initialLength];
            rooms[i].transform.position -= new Vector3(0.5f, 0f, 0.5f);
            rooms[i].PlaceEnemies(manager.variants[val].formationIndex);
        }
    }

    void BuildNav()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
