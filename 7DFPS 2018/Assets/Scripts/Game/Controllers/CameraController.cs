using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Vector3 defaultPosition;

    new public Animation animation;

    private int lastBob = -1;
    public AnimationClip[] bobAnimations;

    new private Camera camera;

    private bool shaking = false;

    public float FOV
    {
        get { return camera.fieldOfView; }
        set { camera.fieldOfView = value; }
    }

    private void Reset()
    {
        defaultPosition = transform.localPosition;
        animation = GetComponent<Animation>();
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!shaking)
            transform.localPosition = defaultPosition;
        shaking = false;
    }

    public void Bob()
    {
        lastBob = (lastBob + 1) % bobAnimations.Length;
        animation.clip = bobAnimations[lastBob];
        animation.Play();
    }

    public void Shake(float intensity)
    {
        shaking = true;
        transform.localPosition = defaultPosition
            + Random.Range(-intensity, intensity) * transform.up
            + Random.Range(-intensity, intensity) * transform.right;
    }
}