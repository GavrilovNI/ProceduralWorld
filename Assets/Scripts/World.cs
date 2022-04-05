#nullable enable
#pragma warning disable CS8618
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class World : MonoBehaviour
{
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private GenerationSettings _generationSettings;

    private readonly Dictionary<Vector3Int, Chunk> _chunks = new();
    private readonly System.Random _random = new();

    public Chunk GetChunk(Vector3Int position)
    {
        if(TryGetChunk(position, out Chunk chunk))
            return chunk;
        else
            throw new System.InvalidOperationException($"Chunk {position} doesn'y exist.");
    }
    public bool TryGetChunk(Vector3Int position, out Chunk chunk) =>
        _chunks.TryGetValue(position, out chunk) && chunk.IsNull() == false;

    public bool HasChunk(Vector3Int position) => TryGetChunk(position, out _);

    protected Chunk CreateChunk(Vector3Int position)
    {
        if(HasChunk(position))
            throw new System.InvalidOperationException($"Chunk {position} already exists.");

        Chunk chunk = GameObject.Instantiate(_chunkPrefab);
        chunk.transform.SetParent(transform);
        chunk.transform.localPosition = _generationSettings.ChunkSize * _generationSettings.Scale * (Vector3)position;
        chunk.name = $"{nameof(Chunk)} {position}";
        chunk.Initialize(position);
        _chunks[position] = chunk;
        return chunk;
    }
    protected Chunk GetOrCreateChunk(Vector3Int position) =>
        TryGetChunk(position, out Chunk chunk) ? chunk : CreateChunk(position);

    public void GenerateChunk(Vector3Int position) =>
        GetOrCreateChunk(position).Generate(_generationSettings);

    public void GenerateChunkParallel(Vector3Int position, UnityThread unityThread, System.Action? callback = null, int actionsInOneThreadNoise = 1, int actionsInOneThreadMesh = 1) =>
        GetOrCreateChunk(position).GenerateParallel(_generationSettings, unityThread, callback, actionsInOneThreadNoise, actionsInOneThreadMesh);

    public void RemoveChunk(Vector3Int position)
    {
        if(TryGetChunk(position, out Chunk chunk) == false)
            throw new System.InvalidOperationException($"Chunk {position} doesn'y exist.");

        _chunks.Remove(position);
        chunk.gameObject.DestroyAnywhere();
    }
}
#pragma warning restore CS8618
#nullable restore
