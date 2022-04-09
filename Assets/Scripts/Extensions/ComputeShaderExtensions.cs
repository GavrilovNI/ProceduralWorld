#nullable enable
using UnityEngine;

namespace UnityExtensions
{
    public static class ComputeShaderExtensions
    {
        public static bool TryGetKernelIndex(this ComputeShader shader, string name, out int kernelIndex)
        {
            if(shader.HasKernel(name))
            {
                kernelIndex = shader.FindKernel(name);
                return true;
            }
            else
            {
                kernelIndex = -1;
                return false;
            }
        }

        public static Vector3Int GetKernelThreadGroupSizes(this ComputeShader shader, int kernelIndex)
        {
            shader.GetKernelThreadGroupSizes(kernelIndex, out uint sizeX, out uint sizeY, out uint sizeZ);
            return new Vector3Int((int)sizeX, (int)sizeY, (int)sizeZ);
        }

        public static void Dispatch(this ComputeShader shader, int kernelIndex, Vector3Int threadGroups) =>
            shader.Dispatch(kernelIndex, threadGroups.x, threadGroups.y, threadGroups.z);
    }
}
#nullable restore
