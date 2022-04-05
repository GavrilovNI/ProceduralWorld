#nullable enable
#pragma warning disable CS8618
using UnityEngine;
using UnityExtensions;

[ExecuteInEditMode]
[RequireComponent(typeof(UnityThread))]
public class TestWorld : World, ITest
{
    private UnityThread _unityThread;
    [SerializeField] private bool _parallel;
    [SerializeField, Min(1)] private int _actionsInOneThreadNoise = 1000;
    [SerializeField, Min(1)] private int _actionsInOneThreadMesh = 1000;
    [SerializeField] private BoundsInt _bounds = new(Vector3Int.zero, Vector3Int.one * 3);

    public void Awake()
    {
        _unityThread = GetComponent<UnityThread>();
    }

    public void Test()
    {
        RemoveAllChunks();

        System.DateTime startTime = System.DateTime.Now;
        if(_parallel)
        {
            int chunksCount = _bounds.ForEachCount();
            _bounds.ForEach(x =>
            {
                GenerateChunkParallel(x, _unityThread, () =>
                {
                    if(System.Threading.Interlocked.Decrement(ref chunksCount) == 0)
                    {
                        System.DateTime endTime = System.DateTime.Now;
                        Debug.Log("Generating Time: " + (endTime - startTime));
                    }
                }, _actionsInOneThreadNoise, _actionsInOneThreadMesh);
            });
        }
        else
        {
            _bounds.ForEach(x =>
            {
                GenerateChunk(x);
            });
            System.DateTime endTime = System.DateTime.Now;
            Debug.Log("Generating Time: " + (endTime - startTime));
        }
    }

    public void Reset()
    {
        RemoveAllChunks();
    }

    public void RemoveAllChunks()
    {
        foreach(var chunk in transform.GetComponentsInChildren<Chunk>())
            GameObject.DestroyImmediate(chunk.gameObject);
    }
}
#pragma warning restore CS8618
#nullable restore

