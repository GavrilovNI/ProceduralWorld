#nullable enable
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityExtensions;

public class MarchingCubesMeshGenerator : IMeshBuilder
{
    private readonly float _cubeSize = 1;
    private readonly MeshData _meshData = new();
    private readonly ConcurrentDictionary<Vector3Int, CreatedMatchingCubeInfo> _cubes = new();

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

        var cubeInfo = MarchingCube.AddFlatCubeToMesh(_meshData, surfaceLevels, surfaceBorder, position.ToFloat() * _cubeSize, _cubeSize);
        if(_cubes.TryAdd(position, cubeInfo) == false)
            throw new System.InvalidOperationException($"Cube on position {position} already exists.");

    }
    private void CreateSmoothCube(float[] surfaceLevels, float surfaceBorder, Vector3Int position, Dictionary<Direction, CreatedMatchingCubeInfo> neighbors)
    {
        var cubeInfo = MarchingCube.AddSmoothCubeToMesh(_meshData, surfaceLevels, surfaceBorder, position.ToFloat() * _cubeSize, _cubeSize, neighbors);
        _cubes.TryAdd(position, cubeInfo);
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

    private static float[] GetBlockSurfaceLevels(float[,,] surfaceLevels, Vector3Int position)
    {
        return new float[8] {
                        surfaceLevels[position.z, position.y, position.x],
                        surfaceLevels[position.z, position.y, position.x + 1],
                        surfaceLevels[position.z + 1, position.y, position.x + 1],
                        surfaceLevels[position.z + 1, position.y, position.x],
                        surfaceLevels[position.z, position.y + 1, position.x],
                        surfaceLevels[position.z, position.y + 1, position.x + 1],
                        surfaceLevels[position.z + 1, position.y + 1, position.x + 1],
                        surfaceLevels[position.z + 1, position.y + 1, position.x]
                    };
    }
    private static System.Action<float[], float, Vector3Int> GetCubeGenerationMethod(MarchingCubesMeshGenerator meshGenerator, bool flat)
    {
        if(flat)
        {
            return meshGenerator.CreateFlatCube;
        }
        else
        {
            return (currentSurfaceLevels, surfaceBorder, position) =>
            {
                Dictionary<Direction, CreatedMatchingCubeInfo> neighbors = new();
                if(position.x > 0)
                    neighbors.Add(Direction.Left, meshGenerator._cubes[position + Vector3Int.left]);
                if(position.y > 0)
                    neighbors.Add(Direction.Down, meshGenerator._cubes[position + Vector3Int.down]);
                if(position.z > 0)
                    neighbors.Add(Direction.Back, meshGenerator._cubes[position + Vector3Int.back]);
                meshGenerator.CreateSmoothCube(currentSurfaceLevels, surfaceBorder, position, neighbors);
            };
        }
    }
    
    //if flat == false, actionsInOneThread is ingnored
    public static void CreateParallel(float[,,] surfaceLevels, float surfaceBorder, float cubeSize, bool flat, System.Action<MarchingCubesMeshGenerator> callback, int actionsInOneThread = 1, CancellationTokenSource? cancellationToken = null)
    {
        Vector3Int size = surfaceLevels.GetLengths() - Vector3Int.one;
        if(size.IsAnyAxis(axis => axis < 1))
            throw new System.ArgumentException(nameof(surfaceLevels), "All dimensions have at least length of 2.");

        MarchingCubesMeshGenerator result = new(cubeSize);
        BoundsInt bounds = size.ToBounds();
        var GenerateCube = GetCubeGenerationMethod(result, flat);

        if(flat == false)
            actionsInOneThread = bounds.ForEachCount();

        bounds.ForEachParallel(position =>
        {
            var currentSurfaceLevels = GetBlockSurfaceLevels(surfaceLevels, position);
            GenerateCube(currentSurfaceLevels, surfaceBorder, position);
        }, () => callback(result), actionsInOneThread, cancellationToken);
    }

    public static MarchingCubesMeshGenerator Create(float[,,] surfaceLevels, float surfaceBorder, float cubeSize, bool flat)
    {
        Vector3Int size = surfaceLevels.GetLengths() - Vector3Int.one;
        if(size.IsAnyAxis(axis => axis < 1))
            throw new System.ArgumentException(nameof(surfaceLevels), "All dimensions have at least length of 2.");

        MarchingCubesMeshGenerator result = new(cubeSize);
        var GenerateCube = GetCubeGenerationMethod(result, flat);

        BoundsInt bounds = size.ToBounds();
        bounds.ForEach(position =>
        {
            var currentSurfaceLevels = GetBlockSurfaceLevels(surfaceLevels, position);
            GenerateCube(currentSurfaceLevels, surfaceBorder, position);
        });

        return result;
    }
}
#nullable restore
