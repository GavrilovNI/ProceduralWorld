#nullable enable
using UnityEngine;

public static class PerlinNoise
{
    private static readonly int[] _permutation = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

    private const int _permutationCountMinus1 = 255; // must be 2^x - 1

    private static readonly Vector3[] _directions = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };

    private const int _directionCountMinusOne = 15; // must be 2^x - 1

    private static float SmoothDistance(float distance) =>
        distance * distance * distance * (distance * (distance * 6f - 15f) + 10f);
    private static float DotProduct(Vector3 a, Vector3 b) =>
        a.x * b.x + a.y * b.y + a.z * b.z;

    public static float Get(Vector3 point, float min, float max)
    {
        float value = Get(point);
        return Mathf.Lerp(min, max, (value + 1) / 2);
    }

    public static float Get01(Vector3 point) => Get(point, 0, 1);

    //returns value in range [-1,1]
    public static float Get(Vector3 point)
    {
        int flooredPointX0 = Mathf.FloorToInt(point.x);
        int flooredPointY0 = Mathf.FloorToInt(point.y);
        int flooredPointZ0 = Mathf.FloorToInt(point.z);

        float distanceX0 = point.x - flooredPointX0;
        float distanceY0 = point.y - flooredPointY0;
        float distanceZ0 = point.z - flooredPointZ0;

        float distanceX1 = distanceX0 - 1f;
        float distanceY1 = distanceY0 - 1f;
        float distanceZ1 = distanceZ0 - 1f;

        flooredPointX0 &= _permutationCountMinus1;
        flooredPointY0 &= _permutationCountMinus1;
        flooredPointZ0 &= _permutationCountMinus1;

        int flooredPointX1 = flooredPointX0 + 1;
        int flooredPointY1 = flooredPointY0 + 1;
        int flooredPointZ1 = flooredPointZ0 + 1;

        int permutationX0 = _permutation[flooredPointX0];
        int permutationX1 = _permutation[flooredPointX1];

        int permutationY00 = _permutation[permutationX0 + flooredPointY0];
        int permutationY01 = _permutation[permutationX0 + flooredPointY1];
        int permutationY10 = _permutation[permutationX1 + flooredPointY0];
        int permutationY11 = _permutation[permutationX1 + flooredPointY1];

        int permutationZ000 = _permutation[permutationY00 + flooredPointZ0];
        int permutationZ001 = _permutation[permutationY00 + flooredPointZ1];
        int permutationZ010 = _permutation[permutationY01 + flooredPointZ0];
        int permutationZ011 = _permutation[permutationY01 + flooredPointZ1];
        int permutationZ100 = _permutation[permutationY10 + flooredPointZ0];
        int permutationZ101 = _permutation[permutationY10 + flooredPointZ1];
        int permutationZ110 = _permutation[permutationY11 + flooredPointZ0];
        int permutationZ111 = _permutation[permutationY11 + flooredPointZ1];

        Vector3 direction000 = _directions[permutationZ000 & _directionCountMinusOne];
        Vector3 direction001 = _directions[permutationZ001 & _directionCountMinusOne];
        Vector3 direction010 = _directions[permutationZ010 & _directionCountMinusOne];
        Vector3 direction011 = _directions[permutationZ011 & _directionCountMinusOne];
        Vector3 direction100 = _directions[permutationZ100 & _directionCountMinusOne];
        Vector3 direction101 = _directions[permutationZ101 & _directionCountMinusOne];
        Vector3 direction110 = _directions[permutationZ110 & _directionCountMinusOne];
        Vector3 direction111 = _directions[permutationZ111 & _directionCountMinusOne];

        float value000 = DotProduct(direction000, new Vector3(distanceX0, distanceY0, distanceZ0));
        float value001 = DotProduct(direction001, new Vector3(distanceX0, distanceY0, distanceZ1));
        float value010 = DotProduct(direction010, new Vector3(distanceX0, distanceY1, distanceZ0));
        float value011 = DotProduct(direction011, new Vector3(distanceX0, distanceY1, distanceZ1));
        float value100 = DotProduct(direction100, new Vector3(distanceX1, distanceY0, distanceZ0));
        float value101 = DotProduct(direction101, new Vector3(distanceX1, distanceY0, distanceZ1));
        float value110 = DotProduct(direction110, new Vector3(distanceX1, distanceY1, distanceZ0));
        float value111 = DotProduct(direction111, new Vector3(distanceX1, distanceY1, distanceZ1));

        float smoothDistanceX = SmoothDistance(distanceX0);
        float smoothDistanceY = SmoothDistance(distanceY0);
        float smoothDistanceZ = SmoothDistance(distanceZ0);


        float notClampedResult = Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(value000, value100, smoothDistanceX),
                                                      Mathf.Lerp(value010, value110, smoothDistanceX),
                                                      smoothDistanceY),
                                           Mathf.Lerp(Mathf.Lerp(value001, value101, smoothDistanceX),
                                                      Mathf.Lerp(value011, value111, smoothDistanceX),
                                                      smoothDistanceY),
                                           smoothDistanceZ);

        return Mathf.Clamp(notClampedResult, -1, 1);
    }
}
#nullable restore
