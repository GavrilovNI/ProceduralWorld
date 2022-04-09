#nullable enable
#pragma warning disable CS8618
using System;
using System.Threading;
using UnityEngine;
using UnityExtensions;

[Serializable]
public class PerlinNoise3D
{
    private readonly PerlinNoise3DSettings _settings;

    public PerlinNoise3D(PerlinNoise3DSettings settings)
    {
        _settings = settings;
    }

    private float TransformSurfaceLevel(Vector3 point, float generatedSurfaceLevel, bool threadSafe)
    {
        AnimationCurve curve = threadSafe ? new AnimationCurve(_settings.SurfaceLevelByHeight.keys) : _settings.SurfaceLevelByHeight;
        float height = point.y;
        if(height <= _settings.MinHeight)
            return curve.Evaluate(0);
        if(height > _settings.MaxHeight)
            return curve.Evaluate(1);

        float t = (height - _settings.MinHeight) / (_settings.MaxHeight - _settings.MinHeight);
        t = Mathf.Clamp01(t);
        return generatedSurfaceLevel * curve.Evaluate(t);
    }

    public float GetPoint(Vector3 point, bool threadSafe = false)
    {
        Vector3 blockPosition = point + _settings.Offset;
        float surfaceLevel = PerlinNoise.Get01(blockPosition * _settings.Frequency + _settings.SeedOffset);
        surfaceLevel = TransformSurfaceLevel(blockPosition, surfaceLevel, threadSafe) * _settings.Amplitude;
        return surfaceLevel;
    }

    public float[,,] GetRange(Vector3 offset, Vector3Int size, CancellationTokenSource? cancellationToken = null)
    {
        float[,,] result = new float[size.z, size.y, size.x];
        size.ToBounds().ForEach(position =>
        {
            result[position.z, position.y, position.x] = GetPoint(position + offset, false);
        }, cancellationToken);
        return result;
    }

    public void GetRangeParallel(Vector3 offset, Vector3Int size, Action<float[,,]> callback, int actionsInOneThread = 100, CancellationTokenSource? cancellationToken = null)
    {
        float[,,] result = new float[size.z, size.y, size.x];
        size.ToBounds().ForEachParallel(position =>
        {
            result[position.z, position.y, position.x] = GetPoint(position + offset, true);
        }, () => callback(result), actionsInOneThread, cancellationToken);
    }
}
#pragma warning restore CS8618
#nullable restore
