using UnityEngine;
using UnityEngine.Tilemaps;

public class FixTilemapCulling : MonoBehaviour
{
    void Start()
    {
        TilemapRenderer tr = GetComponent<TilemapRenderer>();
        if (tr != null)
        {
            // Força o Tilemap a achar que é maior do que realmente é
            tr.detectChunkCullingBounds = TilemapRenderer.DetectChunkCullingBounds.Manual;
            tr.chunkCullingBounds = new Vector3(10, 10, 10);
        }
    }
}