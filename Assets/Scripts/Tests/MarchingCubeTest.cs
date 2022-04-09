#nullable enable
#pragma warning disable CS8618
using UnityEngine;
using UnityExtensions;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MarchingCubeTest : MonoBehaviour, ITest
{
    [SerializeField] private int _cubeIndex = 0;
    [SerializeField, Range(0, 1)] private float _surfaceBorder = 0.5f;
    [SerializeField, Min(0)] private float _size = 1;
    [SerializeField] private bool _flat;
    private MeshFilter _meshFilter;

    public void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Test()
    {
        MeshData meshData = new();
        MarchingCube.AddCubeToMesh(meshData, _cubeIndex, _flat, _surfaceBorder, Vector3.zero, _size);
        _meshFilter.sharedMesh = meshData.Build();
    }

    public void ResetTest()
    {
        _meshFilter.sharedMesh = null;
    }

    public void OnValidate()
    {
        _cubeIndex = Mathf.Clamp(_cubeIndex, 0, 255);
    }
}
#pragma warning restore CS8618
#nullable restore
