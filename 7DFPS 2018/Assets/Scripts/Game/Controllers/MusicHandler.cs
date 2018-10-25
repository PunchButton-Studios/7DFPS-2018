using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public AudioSource[] sources;
    public AudioClip[] clips;

    public float fadeSpeed = 0.5f;
    public float maxVolume = 0.75f;
    private int lastClip = -1;
    private int lastSource = -1;

    public static MusicHandler Main
    {
        get;
        private set;
    }

    private void Reset()
    {
        sources = GetComponents<AudioSource>();
    }

    private void Awake()
    {
        Main = this;
    }

    private void Start()
    {
        PlayNext();
    }

    public void PlayNext()
    {
        lastClip = (lastClip + 1) % clips.Length;
        lastSource = (lastSource + 1) % sources.Length;

        sources[lastSource].clip = clips[lastClip];
        sources[lastSource].Play();
    }

    private void Update()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].volume += (i == lastSource ? 1 : -1) * fadeSpeed * Time.unscaledDeltaTime;
            sources[i].volume = Mathf.Clamp(sources[i].volume, 0, maxVolume);
        }
    }
}