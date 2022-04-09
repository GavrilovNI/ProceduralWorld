#nullable enable
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshSettings", menuName = "ScriptableObjects/MeshSettings")]
public class MeshSettings : ScriptableObject
{
    public const int MaxVerticesOrTrianglesInMesh = 255 * 255;
    public const int MaxVerticesInSmoothCube = 12;
    public const int MaxVerticesInFlatCube = 15;
    public const int MaxTrianglesInCube = 5;

    public static readonly int MaxFlatChunkFlatCubeSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesOrTrianglesInMesh / MaxVerticesInFlatCube, 1f / 3));
    public static readonly int MaxFlatChunkSmoothCubeSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesOrTrianglesInMesh / MaxVerticesInSmoothCube, 1f / 3));
    public static readonly int MaxSmoothChunkSmoothCubeSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesOrTrianglesInMesh / MaxTrianglesInCube, 1f / 3));

    public static readonly int MaxTrianglesInFlatChunkFlatCubes = (int)Mathf.Pow(MaxFlatChunkFlatCubeSize, 3) * MaxTrianglesInCube;

    [SerializeField, Range(0, 1)] private float _surfaceBorder = 0.5f;
    [SerializeField] private int _chunkSize = MaxFlatChunkFlatCubeSize;
    [SerializeField, Min(0)] private float _scale = 1;
    [SerializeField] private bool _flat = false;

    public float SurfaceBorder
    {
        get => _surfaceBorder;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SurfaceBorder)} must be positive.");
            else
                _surfaceBorder = value;
        }
    }
    public int ChunkSize
    {
        get => _chunkSize;
        set
        {
            if(value < 1 || value > MaxFlatChunkSmoothCubeSize)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ChunkSize)} must be in range [1,{MaxFlatChunkSmoothCubeSize}].");
            else
                _chunkSize = value;
        }
    }
    public float Scale
    {
        get => _scale;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Scale)} must be positive.");
            else
                _scale = value;
        }
    }
    public bool Flat
    {
        get => _flat;
        set => _flat = value;
    }

    private void OnValidate()
    {
        if(_chunkSize < 1)
            _chunkSize = 1;

        if(_flat)
        {
            if(_chunkSize > MaxFlatChunkFlatCubeSize)
                _chunkSize = MaxFlatChunkFlatCubeSize;
        }
        else if(_chunkSize > MaxSmoothChunkSmoothCubeSize)
        {
            _chunkSize = MaxSmoothChunkSmoothCubeSize;
        }
    }
}
#nullable restore
