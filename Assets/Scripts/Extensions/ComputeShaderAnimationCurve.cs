#nullable enable
#pragma warning disable CS8618
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ComputeShaderAnimationCurve
{

    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = sizeof(float) * 4)]
    public struct CurveKeyframe
    {
        [FieldOffset(0)]
        public float Time;
        [FieldOffset(sizeof(float))]
        public float Value;
        [FieldOffset(sizeof(float) * 2)]
        public float InTangent;
        [FieldOffset(sizeof(float) * 3)]
        public float OutTangent;

        public CurveKeyframe(float time, float value, float inTangent, float outTangent)
        {
            Time = time;
            Value = value;
            InTangent = inTangent;
            OutTangent = outTangent;
        }

        public static implicit operator CurveKeyframe(Keyframe keyframe) =>
            new(keyframe.time, keyframe.value, keyframe.inTangent, keyframe.outTangent);

    }

    public struct CurveABCD
    {
        public float A, B, C, D;

        public CurveABCD(CurveKeyframe f1, CurveKeyframe f2)
        {
            float p1x = f1.Time;
            float p1y = f1.Value;
            float tp1 = f1.OutTangent;
            float p2x = f2.Time;
            float p2y = f2.Value;
            float tp2 = f2.InTangent;
            float p1xp2 = p1x * p1x;
            float p1xp3 = p1xp2 * p1x;
            float p2xp2 = p2x * p2x;
            float p2xp3 = p2xp2 * p2x;
            float divisor = (p1xp3 - p2xp3 + (3 * p1x * p2x * (p2x - p1x)));
            A = ((tp1 + tp2) * (p1x - p2x) + (p2y - p1y) * 2) / divisor;
            B = (2 * (p2xp2 * tp1 - p1xp2 * tp2) - p1xp2 * tp1 + p2xp2 * tp2 + p1x * p2x * (tp2 - tp1) + 3 * (p1x + p2x) * (p1y - p2y)) / divisor;
            C = (p1xp3 * tp2 - p2xp3 * tp1 + p1x * p2x * (p1x * (2 * tp1 + tp2) - p2x * (tp1 + 2 * tp2)) + 6 * p1x * p2x * (p2y - p1y)) / divisor;
            D = ((p1x * p2xp2 - p1xp2 * p2x) * (p2x * tp1 + p1x * tp2) - p1y * p2xp3 + p1xp3 * p2y + 3 * p1x * p2x * (p2x * p1y - p1x * p2y)) / divisor;
        }

        private bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        public bool IsValid => IsFinite(A) && IsFinite(B) && IsFinite(C) && IsFinite(D);

        public override string ToString()
        {
            return $"{A}, {B}, {C}, {D}";
        }
    }

    public static class ComputeShaderAnimationCurveExtensions
    {
        public const int MaxKeyFramesCountInShader = 16;

        private static CurveKeyframe[] GetKeyframes(this AnimationCurve curve) =>
            Array.ConvertAll(curve.keys, v => (CurveKeyframe)v);

        private static int FindNeededKeyFrameIndex(CurveKeyframe[] keyframes, float t)
        {
            if(keyframes.Length < 2 || t <= keyframes[0].Time)
                return -1;
            if(t >= keyframes[^1].Time)
                return keyframes.Length - 1;
            for(int i = 1; i < keyframes.Length; i++)
            {
                if(t < keyframes[i].Time)
                    return i - 1;
            }
            return keyframes.Length - 1;
        }

        private static float Evaluate(float t, CurveKeyframe[] keyframes)
        {
            int index = FindNeededKeyFrameIndex(keyframes, t);
            if(index == -1)
                return keyframes[0].Value;
            else if(index == keyframes.Length - 1)
                return keyframes[^1].Value;

            var f1 = keyframes[index];
            var f2 = keyframes[index + 1];

            CurveABCD abcd = new(f1, f2);

            if(abcd.IsValid == false)
                return abcd.A > 0 ? f1.Value : f2.Value;

            return Evaluate(t, abcd);
        }

        private static float Evaluate(float t, CurveABCD curveABCD)
        {
            float tp2 = t * t;
            float tp3 = tp2 * t;

            return curveABCD.A * tp3 + curveABCD.B * tp2 + curveABCD.C * t + curveABCD.D;
        }

        public static void SetAnimationCurve(this ComputeShader computeShader, AnimationCurve curve, string keyFramesArrayName, string keyFramesSizeName)
        {
            var keyFrames = curve.GetKeyframes();

            if(keyFrames.Length > MaxKeyFramesCountInShader)
                throw new InvalidOperationException($"Max keyframes in animationcurve to send to compute shader is {MaxKeyFramesCountInShader}.");

            var keyFramesFloats = new float[keyFrames.Length * 4];

            for(int i = 0; i < keyFrames.Length; i++)
            {
                int j = i * 4;
                keyFramesFloats[j] = keyFrames[i].Time;
                keyFramesFloats[j + 1] = keyFrames[i].Value;
                keyFramesFloats[j + 2] = keyFrames[i].InTangent;
                keyFramesFloats[j + 3] = keyFrames[i].OutTangent;
            }
            computeShader.SetInt(keyFramesSizeName, keyFrames.Length);
            computeShader.SetFloats(keyFramesArrayName, keyFramesFloats);

        }
    }
}
#pragma warning restore CS8618
#nullable restore
