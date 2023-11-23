using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class HallWayGenerator : MonoBehaviour
{
    private Mesh _mesh;
    
    private Vector3[] _verts;
    private int[] _triangles;

    public Vector2Int size;
    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        DrawQuad();
        UpdateMesh();
    }

    private void DrawQuad()
    {
        _verts = new Vector3[(size.x + 1) * (size.y + 1)];

        var ind = 0;
        for (var i = 0; i <= size.y; i++)
        {
            for (var j = 0; j <= size.x; j++)
            {
                _verts[ind] = new Vector3(j, 0, i);
                ind++;
            }
        }
        
        _triangles = new int[size.x * size.y * 6];
        var v = 0;
        int tris = 0;

        for (int j = 0; j < size.y; j++)
        {
            for (int i = 0; i < size.x; i++)
            {
                _triangles[tris + 0] = v + 0;
                _triangles[tris + 1] = v + size.x + 1;
                _triangles[tris + 2] = v + 1;
                _triangles[tris + 3] = v + 1;
                _triangles[tris + 4] = v + size.x + 1;
                _triangles[tris + 5] = v + size.x + 2;
    
                v++;
                tris += 6;
            }
            v++;
        }
        
        
    }

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _verts;
        _mesh.triangles = _triangles;
        
        _mesh.RecalculateNormals();
    }
}
