using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    public const int MaxVerticesInMesh = 65535;
    public const int MaxVerticesInCube = 12;
    public const int MaxChunkSize = 65535 / MaxVerticesInCube / MaxVerticesInCube / MaxVerticesInCube;

    private MeshFilter _meshFilter;
    private Vector3Int _chunkPosition;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Initialize(Vector3Int chunkPosition)
    {
        _chunkPosition = chunkPosition;
    }

    public void Generate(GenerationSettings settings)
    {
#if UNITY_EDITOR
        if(Application.isPlaying == false)
            Awake();
#endif

        MeshData data = MeshGenerator.GenerateChunk(_chunkPosition, settings);
        _meshFilter.sharedMesh = data.Build();
    }

}
