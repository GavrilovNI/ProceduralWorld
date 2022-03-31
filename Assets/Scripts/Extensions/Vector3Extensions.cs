#nullable enable
using UnityEngine;

namespace UnityExtensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToFloat(this Vector3Int vector) => new(vector.x, vector.y, vector.z);
    }
}
#nullable disable

