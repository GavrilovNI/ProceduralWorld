#nullable enable
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshSettings", menuName = "ScriptableObjects/MeshSettings")]
public class MeshSettings : ScriptableObject
{
    public const int MaxVerticesOrTrianglesInMesh = 255 * 255;
    public const int MaxVerticesInFlatCube = 12;
    public const int MaxTrianglesInSmoothCube = 4;
    //public static readonly int MaxFlatChunkSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesOrTrianglesInMesh / MaxVerticesInFlatCube, 1f / 3));
    //public static readonly int MaxSmoothChunkSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesOrTrianglesInMesh / MaxTrianglesInSmoothCube, 1f / 3));
    public const int MaxFlatChunkSize = 17;
    public const int MaxSmoothChunkSize = 25;

    [SerializeField, Range(0, 1)] private float _surfaceBorder = 0.5f;
    [SerializeField, Range(1, MaxFlatChunkSize)] private int _chunkSize = MaxFlatChunkSize;
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
            if(value < 1 || value > MaxFlatChunkSize)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ChunkSize)} must be in range [1,{MaxFlatChunkSize}].");
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

}
#nullable restore
