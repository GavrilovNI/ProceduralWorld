#nullable enable
#pragma warning disable CS8618
using System;
using System.Threading;
using UnityEngine;

public class SequentialChunkGenerator : ChunkGenerator
{
    public SequentialChunkGenerator(MeshSettings meshSettings, PerlinNoise3D noise) : base(meshSettings, noise)
    {
    }

    protected override void GenerateChunkMesh(Vector3 chunkOffset, Action<IMeshBuilder> callback, CancellationTokenSource? cancellationToken = null)
    {
        Vector3Int noiseSize = Vector3Int.one * (1 + MeshSettings.ChunkSize);
        float[,,] surfaceLevels = Noise.GetRange(chunkOffset, noiseSize, cancellationToken);
        IMeshBuilder result = MarchingCubesMeshGenerator.Create(surfaceLevels, MeshSettings, cancellationToken);
        if(cancellationToken != null && cancellationToken.IsCancellationRequested)
            return;
        callback(result);
    }
}
#pragma warning restore CS8618
#nullable restore
