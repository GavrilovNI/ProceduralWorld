#nullable enable
#pragma warning disable CS8618
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "ScriptableObjects/PerlinNoise3DSettings")]
public class PerlinNoise3DSettings : ScriptableObject
{
    [SerializeField] private int _seed = 0;

    [SerializeField, Min(0)] private float _amplitude = 1;
    [SerializeField, Min(0)] private float _frequency = Mathf.PI;
    [SerializeField] private Vector3 _offset = Vector3.zero;

    [SerializeField] private AnimationCurve _surfaceLevelByHeight = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private float _minHeight = 0;
    [SerializeField, Min(0)] private float _maxHeight = 256;

    private Vector3? _seedOffset = null;
    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            _seedOffset = null;
        }
    }
    public Vector3 SeedOffset
    {
        get
        {
            if(_seedOffset == null)
            {
                System.Random random = new System.Random(_seed);
                _seedOffset = new(random.Next(-100000, 100000),
                                  random.Next(-100000, 100000),
                                  random.Next(-100000, 100000));
            }
            return _seedOffset.Value;
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
    public Vector3 Offset
    {
        get => _offset;
        set => _offset = value;
    }

    public AnimationCurve SurfaceLevelByHeight => _surfaceLevelByHeight;
    public float MinHeight
    {
        get => _minHeight;
        set
        {
            _minHeight = value;
            if(_maxHeight < _minHeight)
                _maxHeight = value;
        }
    }
    public float MaxHeight
    {
        get => _maxHeight;
        set
        {
            _maxHeight = value;
            if(_minHeight > _maxHeight)
                _minHeight = value;
        }
    }

}
#pragma warning restore CS8618
#nullable restore
