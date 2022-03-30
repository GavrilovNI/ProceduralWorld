using System;
using UnityEngine;

[Serializable]
public struct GenerationSettings
{
    public const int MaxVerticesInMesh = 65535;
    public const int MaxVerticesInCube = 12;
    public const int MaxChunkSize = 65535 / MaxVerticesInCube / MaxVerticesInCube / MaxVerticesInCube;

    [SerializeField] private int _seed;
    [SerializeField, Range(1, MaxChunkSize)] private int _chunkSize;
    [SerializeField, Min(0)] private float _amplitude;
    [SerializeField, Min(0)] private float _frequency;
    [SerializeField, Range(0, 1)] private float _cutMinValue;
    [SerializeField, Min(0)] private float _cubeSize;

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
    public float CutMinValue
    {
        get => _cutMinValue;
        set
        {
            if(value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(CutMinValue)} must be in range [0,1].");
            else
                _cutMinValue = value;
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

    public GenerationSettings(int seed = 0, int chunkSize = MaxChunkSize, float amplitude = 1, float frequency = 0.1f, float cutMinValue = 0.2f, float cubeSize = 1)
    {
        if(chunkSize < 1 || chunkSize > MaxChunkSize)
            throw new ArgumentOutOfRangeException(nameof(chunkSize), $"{nameof(chunkSize)} must be in range [1,{MaxChunkSize}].");
        if(frequency < 0)
            throw new ArgumentOutOfRangeException(nameof(frequency), $"{nameof(frequency)} must be positive.");
        if(amplitude < 0)
            throw new ArgumentOutOfRangeException(nameof(amplitude), $"{nameof(amplitude)} must be positive.");
        if(cutMinValue < 0 || cutMinValue > 1)
            throw new ArgumentOutOfRangeException(nameof(cutMinValue), $"{nameof(cutMinValue)} must be in range [0,1].");
        if(cubeSize < 0)
            throw new ArgumentOutOfRangeException(nameof(cubeSize), $"{nameof(cubeSize)} must be positive.");

        _seed = seed;
        _chunkSize = chunkSize;
        _amplitude = amplitude;
        _frequency = frequency;
        _cutMinValue = cutMinValue;
        _cubeSize = cubeSize;
    }
}
