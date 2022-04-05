#nullable enable
#pragma warning disable CS8618
using UnityEngine;
using UnityExtensions;

[ExecuteInEditMode]
[RequireComponent(typeof(UnityThread))]
public class TestWorld : World, ITest
{
    [SerializeField] private MeshSettings _meshSettings;
    [SerializeField] private PerlinNoise3DSettings _noiseSettings;

    [SerializeField] private bool _parallel;
    [SerializeField, Min(1)] private int _actionsInOneThreadNoise = 1000;
    [SerializeField, Min(1)] private int _actionsInOneThreadMesh = 1000;
    [SerializeField] private BoundsInt _bounds = new(Vector3Int.zero, Vector3Int.one * 3);

    private UnityThread _unityThread;

    public void Awake()
    {
        _unityThread = GetComponent<UnityThread>();
    }

    private ChunkGenerator GetGenerator()
    {
        PerlinNoise3D noise = new(_noiseSettings);
        if(_parallel)
            return new ParallelChunkGenerator(_meshSettings, noise, _actionsInOneThreadNoise, _actionsInOneThreadMesh);
        else
            return new SequentialChunkGenerator(_meshSettings, noise);
    }

    public void Test()
    {
        RemoveAllChunks();

        ChunkGenerator = GetGenerator();
        System.DateTime startTime = System.DateTime.Now;
        int chunksCount = _bounds.ForEachCount();
        _bounds.ForEach(x =>
        {
            GenerateChunk(x, _unityThread, () =>
            {
                if(System.Threading.Interlocked.Decrement(ref chunksCount) == 0)
                {
                    System.DateTime endTime = System.DateTime.Now;
                    Debug.Log("Generating Time: " + (endTime - startTime));
                }
            });
        });
    }

    public void ResetTest()
    {
        RemoveAllChunks();
    }

    public void RemoveAllChunks()
    {
        foreach(var chunk in transform.GetComponentsInChildren<Chunk>())
            chunk.gameObject.DestroyAnywhere();
    }
}
#pragma warning restore CS8618
#nullable restore
