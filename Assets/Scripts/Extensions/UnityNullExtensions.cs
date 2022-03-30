using UnityEngine;

public static class UnityNullExtensions
{
    public static bool IsNull(this MonoBehaviour obj) => obj == null || obj.Equals(null);
}
