using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CameraController : MonoBehaviour
{
    public Vector3 defaultPosition;

    new public Animation animation;

    private int lastBob = -1;
    public AnimationClip[] bobAnimations;

    private void Reset()
    {
        defaultPosition = transform.localPosition;
        animation = GetComponent<Animation>();
    }

    public void Bob()
    {
        lastBob = (lastBob + 1) % bobAnimations.Length;
        animation.clip = bobAnimations[lastBob];
        animation.Play();
    }
}