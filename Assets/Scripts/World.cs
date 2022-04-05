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
    private System.Random _random = new();

    public Chunk? GetChunk(Vector3Int position)
    {
        if(_chunks.ContainsKey(position))
        {
            var chunk = _chunks[position];
            return chunk.IsNull() ? null : _chunks[position];
        }
        else
        {
            return null;
        }
    }
    public bool HasChunk(Vector3Int position) => GetChunk(position) != null;
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

    public void GenerateChunk(Vector3Int position)
    {
        Chunk chunk = GetChunk(position) ?? CreateChunk(position);
        chunk.Generate(_generationSettings);
    }

    public void GenerateChunkParallel(Vector3Int position, UnityThread unityThread, System.Action? callback = null, int actionsInOneThreadNoise = 1, int actionsInOneThreadMesh = 1)
    {
        Chunk chunk = GetChunk(position) ?? CreateChunk(position);
        chunk.GenerateParallel(_generationSettings, unityThread, callback, actionsInOneThreadNoise, actionsInOneThreadMesh);
    }

    public void RemoveChunk(Vector3Int position)
    {
        Chunk? chunk = GetChunk(position);
        if(chunk == null)
            throw new System.InvalidOperationException($"Chunk {position} doesn'y exist.");

        _chunks.Remove(position);
#if UNITY_EDITOR
        if(Application.isPlaying)
            GameObject.Destroy(chunk.gameObject);
        else
            GameObject.DestroyImmediate(chunk.gameObject);
#else
        GameObject.Destroy(chunk.gameObject);
#endif
    }
}
#pragma warning restore CS8618
#nullable disable

