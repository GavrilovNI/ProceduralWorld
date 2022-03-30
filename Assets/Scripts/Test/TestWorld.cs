using UnityEngine;

public class TestWorld : World, ITest
{
    [SerializeField] private BoundsInt _bounds = new(Vector3Int.zero, Vector3Int.one * 3);

    public void Test()
    {
        _bounds.ForEach(x =>
        {
            if(HasChunk(x))
                RemoveChunk(x);
            GenerateChunk(x);
        });
    }

    public void Reset()
    {
        _bounds.ForEach(x =>
        {
            if(HasChunk(x))
                RemoveChunk(x);
        });
    }
}
