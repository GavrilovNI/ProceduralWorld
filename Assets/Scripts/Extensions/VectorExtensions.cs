#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityExtensions
{
    public enum Axis3
    {
        X = 0,
        Y,
        Z
    }

    

    public static class VectorExtensions
    {
        public static float[] GetAxises(this Vector3 vector) =>
            new float[] { vector.x, vector.y, vector.z };
        public static int[] GetAxises(this Vector3Int vector) =>
            new int[] { vector.x, vector.y, vector.z };
        public static float[] GetAxises(this Vector2 vector) =>
            new float[] { vector.x, vector.y };
        public static int[] GetAxises(this Vector2Int vector) =>
            new int[] { vector.x, vector.y };

        public static Vector3 ToV3(this Vector2 vector, float axisValue = 0, Axis3 axisToAdd = Axis3.Z)
        {
            return axisToAdd switch
            {
                Axis3.X => new(axisValue, vector.x, vector.y),
                Axis3.Y => new(vector.x, axisValue, vector.y),
                Axis3.Z => new(vector.x, vector.y, axisValue),
                _ => throw new InvalidOperationException("Unknown axis.")
            };
        }
        public static Vector3Int ToV3(this Vector2Int vector, int axisValue = 0, Axis3 axisToAdd = Axis3.Z)
        {
            return axisToAdd switch
            {
                Axis3.X => new(axisValue, vector.x, vector.y),
                Axis3.Y => new(vector.x, axisValue, vector.y),
                Axis3.Z => new(vector.x, vector.y, axisValue),
                _ => throw new InvalidOperationException("Unknown axis.")
            };
        }
        
        public static Vector2 ToFloat(this Vector2Int vector) => new(vector.x, vector.y);
        public static Vector3 ToFloat(this Vector3Int vector) => new(vector.x, vector.y, vector.z);
        public static Vector3Int ToInt(this Vector3Int vector) => vector.ToInt(Mathf.CeilToInt);
        public static Vector3Int ToInt(this Vector3Int vector, Func<float, int> transform)
        {
            Vector3Int result = new();
            for(int i = 0; i < 3; i++)
                result[i] = transform(vector[i]);
            return result;
        }
        public static Vector2Int ToInt(this Vector2Int vector) => vector.ToInt(Mathf.CeilToInt);
        public static Vector2Int ToInt(this Vector2Int vector, Func<float, int> transform)
        {
            Vector2Int result = new();
            for(int i = 0; i < 2; i++)
                result[i] = transform(vector[i]);
            return result;
        }

        public static Vector3Int ChangeAxises(this Vector3Int vector, Func<int, int> transform) =>
            vector.ChangeAxises((value, _) => transform(value));
        public static Vector3 ChangeAxises(this Vector3 vector, Func<float, float> transform) =>
            vector.ChangeAxises((value, _) => transform(value));
        public static Vector2Int ChangeAxises(this Vector2Int vector, Func<int, int> transform) =>
            vector.ChangeAxises((value, _) => transform(value));
        public static Vector2 ChangeAxises(this Vector2 vector, Func<float, float> transform) =>
            vector.ChangeAxises((value, _) => transform(value));

        public static Vector3Int ChangeAxises(this Vector3Int vector, Func<int, int, int> transform)
        {
            Vector3Int result = new();
            for(int i = 0; i < 3; i++)
                result[i] = transform(vector[i], i);
            return result;
        }
        public static Vector3 ChangeAxises(this Vector3 vector, Func<float, int, float> transform)
        {
            Vector3 result = new();
            for(int i = 0; i < 3; i++)
                result[i] = transform(vector[i], i);
            return result;
        }
        public static Vector2Int ChangeAxises(this Vector2Int vector, Func<int, int, int> transform)
        {
            Vector2Int result = new();
            for(int i = 0; i < 2; i++)
                result[i] = transform(vector[i], i);
            return result;
        }
        public static Vector2 ChangeAxises(this Vector2 vector, Func<float, int, float> transform)
        {
            Vector2 result = new();
            for(int i = 0; i < 2; i++)
                result[i] = transform(vector[i], i);
            return result;
        }

        public static bool IsAnyAxis(this Vector3 vector, Func<float, bool> сondition)
        {
            for(int i = 0; i < 3; i++)
            {
                if(сondition(vector[i]))
                    return true;
            }
            return false;
        }
        public static bool IsAnyAxis(this Vector2 vector, Func<float, bool> сondition)
        {
            for(int i = 0; i < 2; i++)
            {
                if(сondition(vector[i]))
                    return true;
            }
            return false;
        }
        public static bool IsAnyAxis(this Vector3Int vector, Func<int, bool> сondition)
        {
            for(int i = 0; i < 3; i++)
            {
                if(сondition(vector[i]))
                    return true;
            }
            return false;
        }
        public static bool IsAnyAxis(this Vector2Int vector, Func<int, bool> сondition)
        {
            for(int i = 0; i < 2; i++)
            {
                if(сondition(vector[i]))
                    return true;
            }
            return false;
        }

        public static Bounds ToBounds(this Vector3 size) => size.ToBounds(Vector3Int.zero);
        public static Bounds ToBounds(this Vector3 size, Vector3 start)
        {
            if(size.IsAnyAxis(axis => axis < 0))
                throw new ArgumentException(nameof(size), "All axis must be greater or equal 0.");
            return new Bounds(start, size);
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
#nullable restore
