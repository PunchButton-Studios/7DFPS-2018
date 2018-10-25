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
            Vector3 worldVector = new Vector3(v2.x * scale + scale * 0.5f, 0, v2.y * scale + scale * 0.5f);
            chunkRenderers[v2].enabled = Vector3.Distance(player.position, worldVector) <= maxDistance;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(UpdateChunkRendering());
    }

    public void DestroyChildren()
    {
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        chunkRenderers.Clear();
    }
}