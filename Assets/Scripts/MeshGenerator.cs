#nullable enable
using System.Threading.Tasks;
using UnityEngine;

public static class MeshGenerator
{
    public static MarchingCubesMeshGenerator GenerateChunk(Vector3Int chunkPosition, GenerationSettings settings)
    {
        int chunkSize = settings.ChunkSize;
        float amplitude = settings.Amplitude;
        float frequency = settings.Frequency;
        float surfaceBorder = settings.SurfaceBorder;
        float cubeSize = settings.CubeSize;
        Vector3 seedOffset = settings.SeedOffset;

        Vector3 globalChunkPosition = chunkPosition * settings.ChunkSize;

        float[,,] surfaceLevels = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        for(int z = 0; z < chunkSize + 1; z++)
        {
            for(int y = 0; y < chunkSize + 1; y++)
            {
                for(int x = 0; x < chunkSize + 1; x++)
                {
                    Vector3 blockPosition = globalChunkPosition + new Vector3(x, y, z);
                    float surfaceLevel = PerlinNoise.Get01(blockPosition * frequency + seedOffset) * amplitude;
                    surfaceLevel *= settings.GetSurfaceLevelMupltiplier(blockPosition);
                    surfaceLevels[z, y, x] = surfaceLevel;
                }
            }
        }

        return MarchingCubesMeshGenerator.Create(surfaceLevels, surfaceBorder, cubeSize);

        /*MeshData meshData = new();

        for(int z = 0; z < chunkSize; z++)
        {
            for(int y = 0; y < chunkSize; y++)
            {
                for(int x = 0; x < chunkSize; x++)
                {
                    float[] currentSurfaceLevels = new float[8] {
                        surfaceLevels[z, y, x],
                        surfaceLevels[z, y, x + 1],
                        surfaceLevels[z + 1, y, x + 1],
                        surfaceLevels[z + 1, y, x],
                        surfaceLevels[z, y + 1, x],
                        surfaceLevels[z, y + 1, x + 1],
                        surfaceLevels[z + 1, y + 1, x + 1],
                        surfaceLevels[z + 1, y + 1, x]
                    };
                    MarchingCube.AddCubeToMesh(currentSurfaceLevels, surfaceBorder, new Vector3(x, y, z) * cubeSize, cubeSize, meshData);
                }
            }
        }

        return meshData;*/
    }
}
#nullable disable

