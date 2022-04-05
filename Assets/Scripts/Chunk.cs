#nullable enable
#pragma warning disable CS8618
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Vector3Int _chunkIndex;

    private CancellationTokenSource? _generationCancellationToken;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Initialize(Vector3Int chunkPosition)
    {
        _chunkIndex = chunkPosition;
    }

    public void Generate(ChunkGenerator chunkGenerator, UnityThread unityThread, System.Action? callback = null)
    {
        _generationCancellationToken?.Cancel();
        _generationCancellationToken = new();
        chunkGenerator.GenerateChunkMesh(_chunkIndex, meshBuilder =>
        {
            unityThread.Enqueue(() =>
            {
                if(_generationCancellationToken != null && _generationCancellationToken.IsCancellationRequested)
                    return;
                _meshFilter.sharedMesh = meshBuilder.Build();
                callback?.Invoke();
            });
        }, _generationCancellationToken);
    }

    private void OnDisable()
    {
        _generationCancellationToken?.Cancel();
    }
}
#pragma warning restore CS8618
#nullable restore

