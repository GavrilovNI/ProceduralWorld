using System;
using UnityEngine;

public static class BoundsExtensions
{
    public static void ForEach(this BoundsInt bounds, Action<int, int, int> action)
    {
        if(bounds == null)
            throw new ArgumentNullException(nameof(bounds));
        if(action == null)
            return;

        for(int z = bounds.zMin; z < bounds.zMax; z++)
            for(int y = bounds.yMin; y < bounds.yMax; y++)
                for(int x = bounds.xMin; x < bounds.xMax; x++)
                    action.Invoke(x, y, z);
    }

    public static void ForEach(this BoundsInt bounds, Action<Vector3Int> action) =>
        bounds.ForEach((x, y, z) => action(new Vector3Int(x, y, z)));
}
