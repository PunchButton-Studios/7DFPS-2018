using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private int lastClipId = -1;
    public AudioSource audioSource;
    public AudioClip[] clips;

    public float Volume
    {
        get
        {
            return audioSource.volume;
        }
        set
        {
            audioSource.volume = value;
        }
    }

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(int clipId)
    {
        audioSource.PlayOneShot(clips[clipId]);
        lastClipId = clipId;
    }

    public void PlayRandom() => Play(Random.Range(0, clips.Length));

    public void PlayNext()
    {
        lastClipId = (lastClipId + 1) % clips.Length;
        audioSource.PlayOneShot(clips[lastClipId]);
    }
}