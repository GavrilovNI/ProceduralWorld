#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    private readonly List<Vector3> _vertices = new();
    private readonly List<int> _triangleIndexes = new();
    
    public int AddVertex(Vector3 point)
    {
        int index = _vertices.Count;
        _vertices.Add(point);
        return index;
    }

    public void AddTriangle(int[] vertexIndexes)
    {
        if(vertexIndexes.Length != 3)
            throw new ArgumentException(nameof(vertexIndexes), "Length must be 3.");
        _triangleIndexes.AddRange(vertexIndexes);
    }

    public void AddTriangle(int vertex1, int vertex2, int vertex3) => AddTriangle(new int[3] { vertex1, vertex2, vertex3 });
    public void AddTriangleIndex(int vertexIndex) => _triangleIndexes.Add(vertexIndex);

    public Mesh Build()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangleIndexes.ToArray();
        mesh.uv = Array.Empty<Vector2>();
        mesh.RecalculateNormals();
        return mesh;
    }
}
#nullable disable

