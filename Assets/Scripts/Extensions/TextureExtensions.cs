#nullable enable
using UnityEngine;

namespace UnityExtensions
{
    public static class TextureExtensions
    {
        public static Vector2Int GetSize(this Texture texture) =>
            new(texture.width, texture.height);
    }
}
#nullable restore
