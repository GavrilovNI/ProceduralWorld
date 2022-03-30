using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateChunk(Vector3Int chunkPosition, GenerationSettings settings)
    {
        MeshData meshData = new();

        int chunkSize = settings.ChunkSize;
        float amplitude = settings.Amplitude;
        float frequency = settings.Frequency;
        float cutMinValue = settings.CutMinValue;
        float cubeSize = settings.CubeSize;
        Vector3 seedOffset = settings.SeedOffset;

        Vector3 chunkBegin = chunkPosition * settings.ChunkSize;

        float[,,] noise = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];
        for(int z = 0; z < chunkSize + 1; z++)
        {
            for(int y = 0; y < chunkSize + 1; y++)
            {
                for(int x = 0; x < chunkSize + 1; x++)
                {
                    Vector3 blockPosition = chunkBegin + new Vector3(x, y, z);
                    noise[z, y, x] = PerlinNoise.Get(blockPosition * frequency + seedOffset) * amplitude;
                }
            }
        }

        for(int z = 0; z < chunkSize; z++)
        {
            for(int y = 0; y < chunkSize; y++)
            {
                for(int x = 0; x < chunkSize; x++)
                {
                    float[] vertexesInsideCurrent = new float[8] {
                        noise[z, y, x],
                        noise[z, y, x + 1],
                        noise[z + 1, y, x + 1],
                        noise[z + 1, y, x],
                        noise[z, y + 1, x],
                        noise[z, y + 1, x + 1],
                        noise[z + 1, y + 1, x + 1],
                        noise[z + 1, y + 1, x]
                    };
                    MarchingCube.AddCubeToMesh(vertexesInsideCurrent, cutMinValue, new Vector3(x, y, z) * cubeSize, cubeSize, meshData);
                }
            }
        }

        return meshData;
    }
}
