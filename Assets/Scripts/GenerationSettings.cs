#nullable enable
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "ScriptableObjects/GenerationSettings", order = 1)]
public class GenerationSettings : ScriptableObject
{
    public const int MaxVerticesInMesh = 255 * 255;
    public const int MaxVerticesInCube = 12;
    //public static readonly int MaxChunkSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesInMesh / MaxVerticesInCube, 1f / 3));
    public const int MaxChunkSize = 17;

    [SerializeField] private int _seed = 0;
    [SerializeField, Range(1, MaxChunkSize)] private int _chunkSize = MaxChunkSize;
    [SerializeField, Min(0)] private float _amplitude = 1;
    [SerializeField, Min(0)] private float _frequency = Mathf.PI;
    [SerializeField, Min(0)] private float _cubeSize = 1;
    [SerializeField, Range(0, 1)] private float _surfaceBorder = 0.5f;
    [SerializeField] private AnimationCurve _surfaceLevelByHeight = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _minHeight = 5;
    [SerializeField, Min(0)] private float _surfaceHeight = 5;

    public Vector3 SeedOffset
    {
        get
        {
            System.Random random = new System.Random(_seed);
            return new(random.Next(-100000, 100000),
                       random.Next(-100000, 100000),
                       random.Next(-100000, 100000));
        }
    }

    public int Seed
    {
        get => _seed;
        set => _seed = value;
    }
    public int ChunkSize
    {
        get => _chunkSize;
        set
        {
            if(value < 1 || value > MaxChunkSize)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ChunkSize)} must be in range [1,{MaxChunkSize}].");
            else
                _chunkSize = value;
        }
    }
    public float Amplitude
    {
        get => _amplitude;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Amplitude)} must be positive.");
            else
                _amplitude = value;
        }
    }
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
    public float Frequency
    {
        get => _frequency;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Frequency)} must be positive.");
            else
                _frequency = value;
        }
    }
    public float CubeSize
    {
        get => _cubeSize;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(CubeSize)} must be positive.");
            else
                _cubeSize = value;
        }
    }

    public float GetSurfaceLevelMupltiplier(Vector3 point)
    {
        float height = point.y;
        return _surfaceLevelByHeight.Evaluate(Mathf.Clamp01((height - _minHeight) / _surfaceHeight));
    }

    public GenerationSettings()
    {
    }
}
#nullable disable

