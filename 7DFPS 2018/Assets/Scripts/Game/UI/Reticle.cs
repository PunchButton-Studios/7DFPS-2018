using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public bool active;
    public RectTransform[] reticleParts = new RectTransform[4];

    private float transition = 1.0f;
    public float transitionSpeed = 2.5f;
    public float rotation = 25.0f;
    public float resetSpeed = 450.0f;

    private void Update()
    {
        transition = Mathf.Clamp01(transition + transitionSpeed * Time.deltaTime * (active ? -1 : 1));
        
        for(int i = 0; i < reticleParts.Length; i++)
        {
            float zRotation = 90 * i + (90 * transition);
            reticleParts[i].localRotation = Quaternion.Euler(0, 0, zRotation);
        }

        transform.Rotate(0, 0, rotation * Time.deltaTime * (1 - transition));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, transition * resetSpeed * Time.deltaTime);
    }
}