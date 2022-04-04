#nullable enable
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

        for(int z = 0; z < chunkSize + 1; z++)
        {
            for(int y = 0; y < chunkSize + 1; y++)
            {
                for(int x = 0; x < chunkSize + 1; x++)
                {
                    Vector3 blockPosition = new Vector3(x, y, z) + globalChunkPosition;
                    float surfaceLevel = PerlinNoise.Get01(blockPosition * frequency + seedOffset);
                    surfaceLevel = settings.TransformSurfaceLevel(blockPosition, surfaceLevel) * amplitude;
                    surfaceLevels[z, y, x] = surfaceLevel;
                }
            }
        }

        return MarchingCubesMeshGenerator.Create(surfaceLevels, surfaceBorder, scale, flat);
    }
}
#nullable disable

