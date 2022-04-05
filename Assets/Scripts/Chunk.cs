#nullable enable
#pragma warning disable CS8618
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Vector3Int _chunkPosition;

    private CancellationTokenSource _parallelGenerationCancellationToken;

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
        var chunkBuilder = MeshGenerator.GenerateChunk(_chunkPosition, settings);
        _meshFilter.sharedMesh = chunkBuilder.Build();
    }

    public void GenerateParallel(GenerationSettings settings, UnityThread unityThread, System.Action? callback = null, int actionsInOneThreadNoise = 1000, int actionsInOneThreadMesh = 1000)
    {
        _parallelGenerationCancellationToken?.Cancel();
        _parallelGenerationCancellationToken = new();
        MeshGenerator.GenerateChunkParallel(_chunkPosition, settings, chunkBuilder =>
        {
            if(_parallelGenerationCancellationToken.IsCancellationRequested)
                return;
            unityThread.Enqueue(() =>
            {
                if(_parallelGenerationCancellationToken.IsCancellationRequested)
                    return;
                _meshFilter.sharedMesh = chunkBuilder.Build();
                callback?.Invoke();
            });
        }, actionsInOneThreadNoise, actionsInOneThreadMesh, _parallelGenerationCancellationToken);
    }

    private void OnDisable()
    {
        _parallelGenerationCancellationToken?.Cancel();
    }
}
#pragma warning restore CS8618
#nullable disable

