using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler : MonoBehaviour
{
    public Dictionary<Vector2Int, MeshRenderer> chunkRenderers = new Dictionary<Vector2Int, MeshRenderer>();
    public Transform player;
    public float maxDistance = 5.0f;
    public float scale = 1;

    private void Awake() => StartCoroutine(UpdateChunkRendering());

    private IEnumerator UpdateChunkRendering()
    {
        foreach(Vector2Int v2 in chunkRenderers.Keys)
        {
            Vector3 worldVector = new Vector3(v2.x, 0, v2.y);
            chunkRenderers[v2].enabled = Vector3.Distance(player.position / scale, worldVector) <= maxDistance;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(UpdateChunkRendering());
    }
}