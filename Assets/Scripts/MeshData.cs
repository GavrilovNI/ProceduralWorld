using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public readonly List<Vector3> Vertices = new();
    public readonly List<int> TriangleIndexes = new();

    public Mesh Build()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = TriangleIndexes.ToArray();
        mesh.uv = Array.Empty<Vector2>();
        mesh.RecalculateNormals();
        return mesh;
    }
}
