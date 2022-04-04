#nullable enable
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "ScriptableObjects/GenerationSettings", order = 1)]
public class GenerationSettings : ScriptableObject
{
    public const int MaxVerticesInMesh = 255 * 255;
    public const int MaxVerticesInFlatCube = 12;
    //public static readonly int MaxChunkSize = Mathf.FloorToInt(Mathf.Pow(1f * MaxVerticesInMesh / MaxVerticesInFlatCube, 1f / 3));
    public const int MaxChunkSize = 17;

    [SerializeField] private int _seed = 0;
    [SerializeField, Range(1, MaxChunkSize)] private int _chunkSize = MaxChunkSize;
    [SerializeField, Min(0)] private float _amplitude = 1;
    [SerializeField, Min(0)] private float _frequency = Mathf.PI;
    [SerializeField, Min(0)] private float _scale = 1;
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [SerializeField, Range(0, 1)] private float _surfaceBorder = 0.5f;
    [SerializeField] private AnimationCurve _surfaceLevelByHeight = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _minHeight = 5;
    [SerializeField, Min(0)] private float _maxHeight = 5;
    [SerializeField] private bool _flat = false;

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
    public Vector3 Offset
    {
        get => _offset;
        set => _offset = value;
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

    public float TransformSurfaceLevel(Vector3 point, float generatedSurfaceLevel)
    {   
        float height = point.y;
        if(height < _minHeight + 1)
            return 1;
        float t = (height - _minHeight) / (_maxHeight - _minHeight);
        t = Mathf.Clamp01(t);
        return generatedSurfaceLevel * _surfaceLevelByHeight.Evaluate(t);
    }

    public GenerationSettings()
    {
    }
}
#nullable disable

