#nullable enable
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityExtensions
{
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

        public static void ParallelForEach(this BoundsInt bounds, Action<Vector3Int> action)
        {
            int sizeX = bounds.size.x;
            int sizeY = bounds.size.y;
            int sizeZ = bounds.size.z;
            int count = sizeX + sizeY + sizeZ;
            Vector3Int min = bounds.min;

            Parallel.For(0, count, i =>
            {
                int x = i % sizeX;
                i /= sizeX;
                int y = i % sizeY;
                i /= sizeY;
                int z = i % sizeZ;

                action(min + new Vector3Int(x, y, z));
            });
        }
    }
}
#nullable disable

