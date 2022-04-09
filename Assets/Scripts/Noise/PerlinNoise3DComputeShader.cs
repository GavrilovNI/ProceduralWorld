#nullable enable
#pragma warning disable CS8618
using ComputeShaderAnimationCurve;
using UnityEngine;
using UnityExtensions;

public class PerlinNoise3DComputeShader
{
    private readonly PerlinNoise3DSettings _settings;
    private readonly ComputeShader _shader;

    public PerlinNoise3DComputeShader(PerlinNoise3DSettings settings, ComputeShader noiseShader)
    {
        _settings = settings;
        _shader = noiseShader;
    }

    public ComputeBuffer GetRange(Vector3 offset, Vector3Int size)
    {
        _shader.SetFloats("Offset", offset.GetAxises());
        _shader.SetInts("Size", size.GetAxises());
        _shader.SetFloat("Frequency", _settings.Frequency);
        _shader.SetFloats("SeedOffset", _settings.SeedOffset.GetAxises());
        _shader.SetFloat("Amplitude", _settings.Amplitude);
        _shader.SetFloat("MinHeight", _settings.MinHeight);
        _shader.SetFloat("MaxHeight", _settings.MaxHeight);
        _shader.SetAnimationCurve(_settings.SurfaceLevelByHeight, "SurfaceLevelByHeight", "SurfaceLevelByHeightSize");


        int kernelIndex = _shader.FindKernel("GetRange");

        int resultSize = size.x * size.y * size.z;
        ComputeBuffer result = new(resultSize, sizeof(float));
        _shader.SetBuffer(kernelIndex, "SurfaceLevels", result);

        Vector3Int groupSizes = new(8, 8, 8);// _shader.GetKernelThreadGroupSizes(kernelIndex);
        var workSize = groupSizes.ChangeAxises((v, i) => Mathf.CeilToInt(size[i] / (float)v));

        _shader.Dispatch(kernelIndex, workSize);
        return result;
    }
}
#pragma warning restore CS8618
#nullable restore
