#nullable enable
#pragma warning disable CS8618
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityExtensions;

[RequireComponent(typeof(UnityThread))]
public class ComputeShaderChunkGenerator : ChunkGenerator
{
    private readonly ComputeShader _shader;
    private readonly PerlinNoise3DComputeShader _noise;

    public ComputeShaderChunkGenerator(MeshSettings meshSettings, ComputeShader shader, PerlinNoise3DComputeShader noise) : base(meshSettings)
    {
        _shader = shader;
        _noise = noise;
    }

    protected override void GenerateChunkMesh(Vector3 chunkOffset, Action<IMeshBuilder> callback, CancellationTokenSource? cancellationToken)
    {
        int chunkSize = MeshSettings.ChunkSize;
        int pointsCount = chunkSize + 1;

        if(MeshSettings.Flat == false)
            throw new InvalidOperationException($"Non-Flat mesh generation not supported for {nameof(ComputeShaderChunkGenerator)}.");

        Vector3Int noiseSize = Vector3Int.one * pointsCount;


        _shader.SetFloat("Scale", MeshSettings.Scale);
        _shader.SetInt("ChunkSizePlusOne", pointsCount);
        _shader.SetFloat("SurfaceBorder", MeshSettings.SurfaceBorder);

        int kernelIndex = _shader.FindKernel("GenerateFlatCube");

        var surfaceLevelsBuffer = _noise.GetRange(chunkOffset, noiseSize);


        float[] testNoiseData = new float[surfaceLevelsBuffer.count];
        surfaceLevelsBuffer.GetData(testNoiseData);

        _shader.SetBuffer(kernelIndex, "SurfaceLevels", surfaceLevelsBuffer);

        ComputeBuffer trianglesBuffer = new(MeshSettings.MaxTrianglesInFlatChunkFlatCubes, sizeof(float) * 3 * 3, ComputeBufferType.Append);

        _shader.SetBuffer(kernelIndex, "Triangles", trianglesBuffer);

        Vector3Int groupSizes = new(8, 8, 8);// _shader.GetKernelThreadGroupSizes(kernelIndex);
        var workSize = groupSizes.ChangeAxises(v => Mathf.CeilToInt(chunkSize / (float)v));

        _shader.Dispatch(kernelIndex, workSize);

        Vector3[] testData = new Vector3[MeshSettings.MaxTrianglesInFlatChunkFlatCubes * 3];
        trianglesBuffer.GetData(testData);

        surfaceLevelsBuffer.Release();

        ComputeBuffer trianglesCountBuffer = new(1, sizeof(int), ComputeBufferType.Raw);

        ComputeBuffer.CopyCount(trianglesBuffer, trianglesCountBuffer, 0);
        int[] triCountArray = { 0 };
        trianglesCountBuffer.GetData(triCountArray);
        trianglesCountBuffer.Release();

        int trianlgesCount = triCountArray[0];

        Vector3[] triangleVertices = new Vector3[trianlgesCount * 3];

        //float[] triangleVertexAxises = new float[trianlgesCount * 3 * 4];

        trianglesBuffer.GetData(triangleVertices, 0, 0, triangleVertices.Length);
        trianglesBuffer.Release();


        //Buffer.BlockCopy(triangleVertexAxises, 0, triangleVertices, 0, triangleVertexAxises.Length * sizeof(float));

        MeshData result = MeshData.FromTriangles(triangleVertices.ToList());

        callback(result);
    }
}
#pragma warning restore CS8618
#nullable restore
