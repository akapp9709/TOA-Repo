using System.Collections;
using System.Collections.Generic;
using Graphs;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(DungeonBuilder))]
public class Triangulator : MonoBehaviour
{
    public List<Vertex> vertices {get; private set;}
    public List<PF_Delauney.Edge> Edges {get; private set;}
    PF_Delauney _delauney = new PF_Delauney();

    private void OnDrawGizmos()
    {
        if(Edges == null)
            return;

        foreach(var line in Edges)
        {
            var pos1 = line.U.Position + Vector3.up * 10;
            var pos2 = line.V.Position + Vector3.up * 10;

            Gizmos.DrawLine(pos1, pos2);
            Gizmos.DrawCube(pos1, Vector3.one);
        }
    }

    public void BuildTriangulation(List<RoomSpace> content)
    {
        vertices = new List<Vertex>();
        Edges = new List<PF_Delauney.Edge>();
        foreach (var obj in content)
        {
            vertices.Add(new Vertex<RoomSpace>(
                (Vector2)obj.bounds.position + ((Vector2)obj.bounds.size / 2),
                obj
            ));
        }
        
        _delauney = PF_Delauney.Triangulate(vertices);
        Edges = _delauney.Edges;

        Debug.Log($@"
        {vertices.Count} verts
        {Edges.Count} edges");
    }
}
