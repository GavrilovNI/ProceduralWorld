#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class MarchingCubesMeshGenerator
{
    private readonly float _cubeSize = 1;
    private readonly MeshData _meshData = new();
    private readonly Dictionary<Vector3Int, CreatedMatchingCubeInfo> _cubes = new();

    public MarchingCubesMeshGenerator(float cubeSize)
    {
        _cubeSize = cubeSize;
    }

    public Mesh Build() => _meshData.Build();

    public bool HasCube(Vector3Int position) => _cubes.ContainsKey(position);

    public void CreateFlatCube(float[] surfaceLevels, float surfaceBorder, Vector3Int position)
    {
        if(surfaceLevels.Length != 8)
            throw new System.ArgumentException(nameof(surfaceLevels), "Length must be 8");
        if(HasCube(position))
            throw new System.InvalidOperationException($"Cube on position {position} already exists.");

        var cubeInfo = MarchingCube.AddFlatCubeToMesh(_meshData, surfaceLevels, surfaceBorder, position.ToFloat() * _cubeSize, _cubeSize);
        _cubes.Add(position, cubeInfo);
    }
    private void CreateSmoothCube(float[] surfaceLevels, float surfaceBorder, Vector3Int position, Dictionary<Direction, CreatedMatchingCubeInfo> neighbors)
    {
        var cubeInfo = MarchingCube.AddSmoothCubeToMesh(_meshData, surfaceLevels, surfaceBorder, position.ToFloat() * _cubeSize, _cubeSize, neighbors);
        _cubes.Add(position, cubeInfo);
    }

    public void CreateSmoothCube(float[] surfaceLevels, float surfaceBorder, Vector3Int position)
    {
        if(surfaceLevels.Length != 8)
            throw new System.ArgumentException(nameof(surfaceLevels), "Length must be 8");
        if(HasCube(position))
            throw new System.InvalidOperationException($"Cube on position {position} already exists.");

        Direction[] directions = new Direction[]
        {
            Direction.Up,
            Direction.Down,
            Direction.Right,
            Direction.Left,
            Direction.Forward,
            Direction.Back,
            Direction.UpRight,
            Direction.UpLeft,
            Direction.UpForward,
            Direction.UpBack,
            Direction.DownRight,
            Direction.DownLeft,
            Direction.DownForward,
            Direction.DownBack,
            Direction.ForwardLeft,
            Direction.ForwardRight,
            Direction.BackLeft,
            Direction.BackRight
        };
        Dictionary<Direction, CreatedMatchingCubeInfo> neighbors = new();

        foreach(Direction direction in directions)
        {
            if(_cubes.TryGetValue(position + direction.ToVector(), out var cube))
                neighbors.Add(direction, cube);
        }
        CreateSmoothCube(surfaceLevels, surfaceBorder, position, neighbors);
    }

    public static MarchingCubesMeshGenerator Create(float[,,] surfaceLevels, float surfaceBorder, float cubeSize, bool flat)
    {
        int zLength = surfaceLevels.GetLength(0) - 1;
        int yLength = surfaceLevels.GetLength(1) - 1;
        int xLength = surfaceLevels.GetLength(2) - 1;
        if(zLength < 1 || yLength < 1 || xLength < 1)
            throw new System.ArgumentException(nameof(surfaceLevels), "All dimensions have at least length of 2.");

        MarchingCubesMeshGenerator result = new(cubeSize);

        float[] GetSurfaceLevels(Vector3Int position) => new float[8] {
                        surfaceLevels[position.z, position.y, position.x],
                        surfaceLevels[position.z, position.y, position.x + 1],
                        surfaceLevels[position.z + 1, position.y, position.x + 1],
                        surfaceLevels[position.z + 1, position.y, position.x],
                        surfaceLevels[position.z, position.y + 1, position.x],
                        surfaceLevels[position.z, position.y + 1, position.x + 1],
                        surfaceLevels[position.z + 1, position.y + 1, position.x + 1],
                        surfaceLevels[position.z + 1, position.y + 1, position.x]
                    };

        System.Action<float[], float, Vector3Int> generateCube;
        if(flat)
        {
            generateCube = result.CreateFlatCube;
        }
        else
        {
            generateCube = (currentSurfaceLevels, surfaceBorder, position) =>
            {
                Dictionary<Direction, CreatedMatchingCubeInfo> neighbors = new();
                if(position.x > 0)
                    neighbors.Add(Direction.Left, result._cubes[position + Vector3Int.left]);
                if(position.y > 0)
                    neighbors.Add(Direction.Down, result._cubes[position + Vector3Int.down]);
                if(position.z > 0)
                    neighbors.Add(Direction.Back, result._cubes[position + Vector3Int.back]);
                result.CreateSmoothCube(currentSurfaceLevels, surfaceBorder, position, neighbors);
            };
        }

        for(int z = 0; z < zLength; z++)
        {
            for(int y = 0; y < yLength; y++)
            {
                for(int x = 0; x < xLength; x++)
                {
                    Vector3Int position = new(x, y, z);
                    var currentSurfaceLevels = GetSurfaceLevels(position);
                    generateCube(currentSurfaceLevels, surfaceBorder, position);
                }
            }
        }

        return result;
    }
}
#nullable disable

