using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelDropEffect : MonoBehaviour
{
    public float speed;
    private float offset;
    public string textureTarget;
    private Material material;

    private void Start()
    {
        material = new Material(GetComponent<MeshRenderer>().sharedMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    private void Update()
    {
        offset += speed * Time.unscaledDeltaTime;
        material.SetTextureOffset(textureTarget, new Vector2(0, offset));
    }
}