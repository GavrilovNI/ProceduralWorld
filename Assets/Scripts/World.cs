using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private GenerationSettings _generationSettings;

    private readonly Dictionary<Vector3Int, Chunk> _chunks = new();
    private System.Random _random = new System.Random();

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
    private Chunk CreateChunk(Vector3Int position)
    {
        if(HasChunk(position))
            throw new System.InvalidOperationException($"Chunk {position} already exists.");

        Chunk chunk = GameObject.Instantiate(_chunkPrefab);
        chunk.transform.position = (Vector3)position * _generationSettings.ChunkSize * _generationSettings.CubeSize;
        chunk.transform.SetParent(transform);
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

    public void RemoveChunk(Vector3Int position)
    {
        Chunk chunk = GetChunk(position);
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
