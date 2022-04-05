#nullable enable
using UnityEngine;

namespace UnityExtensions
{
    public static class UnityObjectExtensions
    {
        public static void DestroyAnywhere(this UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
#else
        GameObject.Destroy(chunk.gameObject);
#endif
        }
    }
}
#nullable restore
