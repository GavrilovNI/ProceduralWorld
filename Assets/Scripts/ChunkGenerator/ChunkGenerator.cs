#nullable enable
#pragma warning disable CS8618
using System;
using System.Threading;
using UnityEngine;
using UnityExtensions;

public abstract class ChunkGenerator
{
    protected MeshSettings MeshSettings;
    protected PerlinNoise3D Noise;

    public ChunkGenerator(MeshSettings meshSettings, PerlinNoise3D noise)
    {
        MeshSettings = meshSettings;
        Noise = noise;
    }

    public Vector3 GetRealChunkPosition(Vector3Int chunkIndex) =>
        MeshSettings.ChunkSize * MeshSettings.Scale * chunkIndex.ToFloat();

    protected abstract void GenerateChunkMesh(Vector3 chunkOffset, Action<IMeshBuilder> callback, CancellationTokenSource? cancellationToken);

    public void GenerateChunkMesh(Vector3Int chunkIndex, Action<IMeshBuilder> callback, CancellationTokenSource? cancellationToken = null)
    {
        Vector3 chunkOffset = chunkIndex.ToFloat() * MeshSettings.ChunkSize;
        GenerateChunkMesh(chunkOffset, callback, cancellationToken);
    }
}
#pragma warning restore CS8618
#nullable restore
