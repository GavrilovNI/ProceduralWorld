#nullable enable
#pragma warning disable CS8618
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Vector3Int _chunkIndex;

    private CancellationTokenSource? _generationCancellationToken;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Initialize(Vector3Int chunkPosition)
    {
        _chunkIndex = chunkPosition;
    }

    public void Generate(ChunkGenerator chunkGenerator, UnityThread unityThread, System.Action? callback = null)
    {
        _generationCancellationToken?.Cancel();
        _generationCancellationToken = new();
        chunkGenerator.GenerateChunkMesh(_chunkIndex, meshBuilder =>
        {
            var xx = _generationCancellationToken;
            string nameSave = name;
            string t0 = xx == null ? "null" : xx.IsCancellationRequested.ToString();
            string t20 = xx?.Token == null ? "null" : xx.Token.IsCancellationRequested.ToString();
            //Debug.Log($"BeforeSet {name} {t0} {t20} {System.DateTime.Now:mm:ss.FFFF}");
            unityThread.Enqueue(() =>
            {
                if(_generationCancellationToken.IsCancellationRequested)
                    return;

                string t = _generationCancellationToken == null ? "null" : _generationCancellationToken.IsCancellationRequested.ToString();
                string t2 = _generationCancellationToken?.Token == null ? "null" : _generationCancellationToken.Token.IsCancellationRequested.ToString();

                try
                {
                    //Debug.Log($"Set {name} {t} {t2} {object.ReferenceEquals(xx, _generationCancellationToken)}");
                }
                catch(MissingReferenceException)
                {
                    //Debug.Log($"Set Error {nameSave} {t} {t2}");
                }
                _meshFilter.sharedMesh = meshBuilder.Build();
                callback?.Invoke();
            });
        }, _generationCancellationToken);
    }

    private void OnDisable()
    {
        _generationCancellationToken?.Cancel();
        string t = _generationCancellationToken == null ? "null" : _generationCancellationToken.IsCancellationRequested.ToString();
        string t2 = _generationCancellationToken?.Token == null ? "null" : _generationCancellationToken.Token.IsCancellationRequested.ToString();
        //Debug.Log($"Disable {name} {t} {t2}");
    }

    private void OnEnable()
    {
        _generationCancellationToken = new();
        string t = _generationCancellationToken == null ? "null" : _generationCancellationToken.IsCancellationRequested.ToString();
        string t2 = _generationCancellationToken?.Token == null ? "null" : _generationCancellationToken.Token.IsCancellationRequested.ToString();
        //Debug.Log($"Enable {name} {t} {t2}");
    }
}
#pragma warning restore CS8618
#nullable restore

