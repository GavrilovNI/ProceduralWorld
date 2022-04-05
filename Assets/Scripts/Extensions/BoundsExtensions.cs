#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityExtensions
{
    public static class BoundsExtensions
    {
        public static int ForEachCount(this BoundsInt bounds, int actionsInOneThread = 1)
        {
            if(actionsInOneThread < 1)
                throw new ArgumentOutOfRangeException(nameof(actionsInOneThread), "Must be more than 0.");
            int count = bounds.size.x * bounds.size.y * bounds.size.z;
            return (count + actionsInOneThread - 1) / actionsInOneThread;
        }

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

        public static void ForEachParallel(this BoundsInt bounds, Action<Vector3Int> action, Action? callback = null, int actionsInOneThread = 100, CancellationTokenSource? cancellationToken = null)
        {
            if(bounds == null)
                throw new ArgumentNullException(nameof(bounds));
            if(actionsInOneThread < 1)
                throw new ArgumentOutOfRangeException(nameof(actionsInOneThread), "Must be more than 0.");
            if(action == null)
                return;

            if(cancellationToken == null)
                cancellationToken = new();

            int count = bounds.ForEachCount();
            if(count == 0)
            {
                callback?.Invoke();
                return;
            }

            Vector3Int IndexToVector(int value)
            {
                Vector3Int result = new();
                result.x = value % bounds.size.x;
                value /= bounds.size.x;
                result.y = value % bounds.size.y;
                value /= bounds.size.x;
                result.z = value % bounds.size.z;
                return result;
            };
            Vector3Int IncrementVector(Vector3Int vector)
            {
                vector.x++;
                if(vector.x == bounds.size.x)
                {
                    vector.x = 0;
                    vector.y++;
                    if(vector.y == bounds.size.y)
                    {
                        vector.y = 0;
                        vector.z++;
                    }
                }
                return vector;
            };

            int leftCount = bounds.ForEachCount(actionsInOneThread);
            int threadCount = leftCount;
            void ThreadAction(object threadIndex)
            {
                int begin = (int)threadIndex * actionsInOneThread;
                int end = Math.Min(begin + actionsInOneThread, count);
                Vector3Int vector = IndexToVector(begin);
                for(int i = begin; i < end; i++)
                {
                    if(cancellationToken.IsCancellationRequested)
                        return;
                    action(vector);
                    vector = IncrementVector(vector);
                }
                if(Interlocked.Decrement(ref leftCount) == 0)
                    callback?.Invoke();
            }

            for(int i = 0; i < threadCount; i++)
                ThreadPool.QueueUserWorkItem(ThreadAction, i);
        }
    }
}
#nullable disable

