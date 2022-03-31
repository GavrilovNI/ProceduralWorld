#nullable enable
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
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

        var chunkBuilder = MeshGenerator.GenerateChunk(_chunkPosition, settings);
        _meshFilter.sharedMesh = chunkBuilder.Build();
    }

}
#nullable disable

