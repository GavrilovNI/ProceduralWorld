#nullable enable
using UnityEngine;
using UnityExtensions;

public class TestWorld : World, ITest
{
    [SerializeField] private BoundsInt _bounds = new(Vector3Int.zero, Vector3Int.one * 3);

    public void Test()
    {
        RemoveAllChunks();

        System.DateTime startTime = System.DateTime.Now;
        _bounds.ForEach(x =>
        {
            GenerateChunk(x);
        });
        System.DateTime endTime = System.DateTime.Now;
        Debug.Log("Generating Time: " + (endTime - startTime));
    }

    public void Reset()
    {
        RemoveAllChunks();
    }

    public void RemoveAllChunks()
    {
        foreach(var chunk in transform.GetComponentsInChildren<Chunk>())
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
#nullable disable

