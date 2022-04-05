using System;
using UnityEngine;

namespace UnityExtensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToFloat(this Vector3Int vector) => new(vector.x, vector.y, vector.z);

        public static bool IsAnyAxis(this Vector3Int vector, Func<int, bool> сondition)
        {
            for(int i = 0; i < 3; i++)
            {
                if(сondition(vector[i]))
                    return true;
            }
            return false;
        }

        public static BoundsInt ToBounds(this Vector3Int size) => size.ToBounds(Vector3Int.zero);
        public static BoundsInt ToBounds(this Vector3Int size, Vector3Int start)
        {
            if(size.IsAnyAxis(axis => axis < 0))
                throw new ArgumentException(nameof(size), "All axis must be greater or equal 0.");
            return new BoundsInt(start, size);
        }

    }
}
