#nullable enable
#pragma warning disable CS8618
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class World : MonoBehaviour
{
    [SerializeField] private Chunk _chunkPrefab;

    private readonly Dictionary<Vector3Int, Chunk> _chunks = new();
    private readonly System.Random _random = new();

    protected ChunkGenerator ChunkGenerator;

    public void Initialize(ChunkGenerator chunkGenerator)
    {
        if(ChunkGenerator != null)
            throw new System.InvalidOperationException("Already initilized.");
        else
            ChunkGenerator = chunkGenerator;
    }

    public Chunk GetChunk(Vector3Int position)
    {
        if(TryGetChunk(position, out Chunk chunk))
            return chunk;
        else
            throw new System.InvalidOperationException($"Chunk {position} doesn'y exist.");
    }
    public bool TryGetChunk(Vector3Int index, out Chunk chunk) =>
        _chunks.TryGetValue(index, out chunk) && chunk.IsNull() == false;

    public bool HasChunk(Vector3Int index) => TryGetChunk(index, out _);

    protected Chunk CreateChunk(Vector3Int index)
    {
        if(HasChunk(index))
            throw new System.InvalidOperationException($"Chunk {index} already exists.");

        Chunk chunk = GameObject.Instantiate(_chunkPrefab);
        chunk.transform.SetParent(transform);
        chunk.transform.localPosition = ChunkGenerator.GetRealChunkPosition(index);
        chunk.name = $"{nameof(Chunk)} {index} {System.DateTime.Now:mm:ss.FFFF}";
        chunk.Initialize(index);
        _chunks[index] = chunk;
        return chunk;
    }
    protected Chunk GetOrCreateChunk(Vector3Int position) =>
        TryGetChunk(position, out Chunk chunk) ? chunk : CreateChunk(position);

    public void GenerateChunk(Vector3Int position, UnityThread unityThread, System.Action? callback = null) =>
        GetOrCreateChunk(position).Generate(ChunkGenerator, unityThread, callback);

    public void RemoveChunk(Vector3Int index)
    {
        if(TryGetChunk(index, out Chunk chunk) == false)
            throw new System.InvalidOperationException($"Chunk {index} doesn'y exist.");

        _chunks.Remove(index);
        chunk.gameObject.DestroyAnywhere();
    }
}
#pragma warning restore CS8618
#nullable restore
