#nullable enable
using UnityEngine;

namespace UnityExtensions
{
    public static class ArrayExtensions
    {
        public static Vector3Int GetLengths<T>(this T[,,] array)
        {
            if(array == null)
                return Vector3Int.zero;
            return new Vector3Int(array.GetLength(2), array.GetLength(1), array.GetLength(0));
        }

        public static Vector2Int GetLengths<T>(this T[,] array)
        {
            if(array == null)
                return Vector2Int.zero;
            return new Vector2Int(array.GetLength(1), array.GetLength(0));
        }
    }
}
#nullable restore
