#nullable enable
#pragma warning disable CS8618
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TestChunk : Chunk, ITest
{
    [SerializeField] private GenerationSettings _settings;

    public void Test()
    {
        Generate(_settings);
    }

    public void Reset()
    {
        GetComponent<MeshFilter>().sharedMesh = null;
    }
}
#pragma warning restore CS8618
#nullable disable

