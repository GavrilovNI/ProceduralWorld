#nullable enable
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityExtensions;

public static class MeshGenerator
{
    public static MarchingCubesMeshGenerator GenerateChunk(Vector3Int chunkPosition, GenerationSettings settings)
    {
        int chunkSize = settings.ChunkSize;
        float amplitude = settings.Amplitude;
        float frequency = settings.Frequency;
        float surfaceBorder = settings.SurfaceBorder;
        float scale = settings.Scale;
        Vector3 offset = settings.Offset;
        Vector3 seedOffset = settings.SeedOffset;
        bool flat = settings.Flat;

        Vector3 globalChunkPosition = chunkPosition.ToFloat() * settings.ChunkSize + offset;
        float[,,] surfaceLevels = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        new BoundsInt(Vector3Int.zero, Vector3Int.one * (chunkSize + 1)).ForEach(localBlockPosition =>
        {
            Vector3 blockPosition = localBlockPosition + globalChunkPosition;
            float surfaceLevel = PerlinNoise.Get01(blockPosition * frequency + seedOffset);
            surfaceLevel = settings.TransformSurfaceLevel(blockPosition, surfaceLevel) * amplitude;
            surfaceLevels[localBlockPosition.z, localBlockPosition.y, localBlockPosition.x] = surfaceLevel;
        });

        return MarchingCubesMeshGenerator.Create(surfaceLevels, surfaceBorder, scale, flat);
    }

    public static void GenerateChunkParallel(Vector3Int chunkPosition, GenerationSettings settings, System.Action<MarchingCubesMeshGenerator> callback, int actionsInOneThreadNoise, int actionsInOneThreadMesh, CancellationTokenSource? cancellationToken = null)
    {
        int chunkSize = settings.ChunkSize;
        float amplitude = settings.Amplitude;
        float frequency = settings.Frequency;
        float surfaceBorder = settings.SurfaceBorder;
        float scale = settings.Scale;
        Vector3 offset = settings.Offset;
        Vector3 seedOffset = settings.SeedOffset;
        bool flat = settings.Flat;

        Vector3 globalChunkPosition = chunkPosition.ToFloat() * settings.ChunkSize + offset;
        float[,,] surfaceLevels = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        new BoundsInt(Vector3Int.zero, Vector3Int.one * (chunkSize + 1)).ForEachParallel(localBlockPosition =>
        {
            Vector3 blockPosition = localBlockPosition + globalChunkPosition;
            float surfaceLevel = PerlinNoise.Get01(blockPosition * frequency + seedOffset);
            surfaceLevel = settings.TransformSurfaceLevel(blockPosition, surfaceLevel) * amplitude;
            surfaceLevels[localBlockPosition.z, localBlockPosition.y, localBlockPosition.x] = surfaceLevel;
        }, () =>
        {
            MarchingCubesMeshGenerator.CreateParallel(surfaceLevels, surfaceBorder, scale, flat, callback, actionsInOneThreadMesh, cancellationToken);
        }, actionsInOneThreadNoise, cancellationToken);
    }
}
#nullable restore
