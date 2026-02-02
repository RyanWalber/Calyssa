using UnityEngine;
using UnityEngine.Tilemaps;

public class FixTilemapCulling : MonoBehaviour
{
    void Start()
    {
        TilemapRenderer tr = GetComponent<TilemapRenderer>();
        if (tr != null)
        {
            tr.detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Manual;
            tr.chunkCullingBounds = new Vector3(10, 10, 10);
        }
    }
}