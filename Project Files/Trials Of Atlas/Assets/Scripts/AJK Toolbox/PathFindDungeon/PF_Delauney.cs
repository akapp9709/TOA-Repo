/*
 * Adapted from https://github.com/Bl4ckb0ne/delaunay-triangulation

    Copyright (c) 2015-2019 Simon Zeni (simonzeni@gmail.com)


    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:


    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.


    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using Graphs;
using UnityEngine;

//Currently Two-Dimensional - planned exploration of 3D Delaunay in future
public class PF_Delauney
{
    public class Triangle : IEquatable<Triangle> {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        
        public bool IsBad { get; set; }

        public Triangle() {

        }

        public Triangle(Vertex a, Vertex b, Vertex c) {
            A = a;
            B = b;
            C = c;
        }

        public bool ContainsVertex(Vector3 v) {
            return Vector3.Distance(v, A.Position) < 0.01f
                || Vector3.Distance(v, B.Position) < 0.01f
                || Vector3.Distance(v, C.Position) < 0.01f;
        }

        public bool CircumCircleContains(Vector3 v) {
            Vector3 a = A.Position;
            Vector3 b = B.Position;
            Vector3 c = C.Position;

            float ab = a.sqrMagnitude;
            float cd = b.sqrMagnitude;
            float ef = c.sqrMagnitude;

            float circumX = (ab * (c.y - b.y) + cd * (a.y - c.y) + ef * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
            float circumY = (ab * (c.x - b.x) + cd * (a.x - c.x) + ef * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));

            Vector3 circum = new Vector3(circumX / 2, circumY / 2);
            float circumRadius = Vector3.SqrMagnitude(a - circum);
            float dist = Vector3.SqrMagnitude(v - circum);
            return dist <= circumRadius;
        }

        public static bool operator ==(Triangle left, Triangle right) {
            return (left.A == right.A || left.A == right.B || left.A == right.C)
                && (left.B == right.A || left.B == right.B || left.B == right.C)
                && (left.C == right.A || left.C == right.B || left.C == right.C);
        }

        public static bool operator !=(Triangle left, Triangle right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Triangle t) {
                return this == t;
            }

            return false;
        }

        public bool Equals(Triangle t) {
            return this == t;
        }

        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
    }

    public class Edge {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        public bool IsBad { get; set; }

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                   && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }

        public static bool AlmostEqual(Edge left, Edge right) {
            return PF_Delauney.AlmostEqual(left.U, right.U) && PF_Delauney.AlmostEqual(left.V, right.V)
                   || PF_Delauney.AlmostEqual(left.U, right.V) && PF_Delauney.AlmostEqual(left.V, right.U);
        }
    }

    private static bool AlmostEqual(float x, float y)
    {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2 
               || Mathf.Abs(x - y ) < float.MinValue;
    }

    static bool AlmostEqual(Vertex left, Vertex right) {
        return AlmostEqual(left.Position.x, right.Position.x) && AlmostEqual(left.Position.y, right.Position.y);
    }
    
    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }

    public PF_Delauney()
    {
        Edges = new List<Edge>();
        Vertices = new List<Vertex>();
    }

    public static PF_Delauney Triangulate(List<Vertex> vertices)
    {
        var delauney = new PF_Delauney();
        delauney.Vertices = new List<Vertex>(vertices);
        delauney.Triangulate();

        return delauney;
    }

    private void Triangulate()
    {
        Triangles = new List<Triangle>();
        
        var minX = Vertices[0].Position.x;
        var minY = Vertices[0].Position.y;
        var maxX = minX;
        var maxY = minY;

        foreach (var vert in Vertices)
        {
            var pos = vert.Position;
            if (pos.x < minX)
                minX = pos.x;
            if (pos.x > maxX)
                maxX = pos.x;
            if (pos.y < minY)
                minY = pos.y;
            if (pos.y > maxY)
                maxY = pos.y;
        }
        
        //Create Super Triangle - A triangle that encompasses all points needed
        var dX = maxX - minX;
        var dY = maxY - minY;
        var deltaMax = Mathf.Max(dX, dY) * 2;
        
        var p1 = new Vertex(new Vector2(
            minX - 1,
            minY - 1)
        );
        var p2 = new Vertex(new Vector2(
            minX - 1,
            maxY + deltaMax)
        );
        var p3 = new Vertex(new Vector2(
            maxX + deltaMax,
            minY - 1)
        );
        
        Triangles.Add(new Triangle(p1, p2, p3));
        
        //Loop through vertices to find Triangulation
        foreach (var vert in Vertices)
        {
            var polygon = new List<Edge>();                                                                             //current polygon under question

            foreach (var t in Triangles)                                                                          //check if point lies within a circumcircle of any already existing triangles 
            {
                if (t.CircumCircleContains(vert.Position))                                                              
                {
                    t.IsBad = true;
                    polygon.Add(new Edge(t.A, t.B));
                    polygon.Add(new Edge(t.B, t.C));
                    polygon.Add(new Edge(t.C, t.A));
                }
            }
            Triangles.RemoveAll((Triangle t) => t.IsBad);                                                          // Remove all triangles affected by the current vertex

            for (int i = 0; i < polygon.Count; i++)                                                                         //Check for redundant Edges
            {
                for (int j = i+1; j < polygon.Count; j++)
                {
                    if (Edge.AlmostEqual(polygon[i], polygon[j]))
                    {
                        polygon[i].IsBad = true;
                        polygon[j].IsBad = true;
                    }
                }
            }

            polygon.RemoveAll((Edge e) => e.IsBad);                                                                // Remove flagged edges

            foreach (var edge in polygon)                                                                               // Add new triangles for every affected point to include new vertex
            {
                Triangles.Add(new Triangle(edge.U, edge.V, vert));
            }
        }
        
        Triangles.RemoveAll((Triangle t) => t.ContainsVertex(p1.Position) || t.ContainsVertex(p2.Position) || t.ContainsVertex(p3.Position));
        HashSet<Edge> edgeSet = new HashSet<Edge>();

        foreach (var t in Triangles)
        {
            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);

            if (edgeSet.Add(ab))
            {
                Edges.Add(ab);
            }
            if (edgeSet.Add(bc))
            {
                Edges.Add(bc);
            }
            if (edgeSet.Add(ca))
            {
                Edges.Add(ca);
            }
        }
    }
}
