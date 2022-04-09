#nullable enable
#pragma warning disable CS8618
using System;
using System.Threading;
using UnityEngine;

public class ParallelChunkGenerator : ChunkGenerator
{
    private readonly PerlinNoise3D _noise;
    private readonly int _actionsInOneThreadNoise;
    private readonly int _actionsInOneThreadMesh;

    public ParallelChunkGenerator(MeshSettings meshSettings, PerlinNoise3D noise,
        int actionsInOneThreadNoise, int actionsInOneThreadMesh) : base(meshSettings)
    {
        _noise = noise;
        _actionsInOneThreadNoise = actionsInOneThreadNoise;
        _actionsInOneThreadMesh = actionsInOneThreadMesh;
    }
    protected override void GenerateChunkMesh(Vector3 chunkOffset, Action<IMeshBuilder> callback, CancellationTokenSource? cancellationToken = null)
    {
        Vector3Int noiseSize = Vector3Int.one * (1 + MeshSettings.ChunkSize);
        _noise.GetRangeParallel(chunkOffset, noiseSize,
                               surfaceLevels => MarchingCubesMeshGenerator.CreateParallel(surfaceLevels, MeshSettings,
                                                                                          callback, _actionsInOneThreadMesh,
                                                                                          cancellationToken),
                               _actionsInOneThreadNoise, cancellationToken);
    }
}
#pragma warning restore CS8618
#nullable restore
