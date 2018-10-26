using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeField : MonoBehaviour
{
    private bool charging = false;
    public float chargeSpeed = 0.01f;
    public AudioSource audioSource;
    public float audioDecreaseSpeed = 1.0f;

    private void Awake()
    {
        GameManager.Main.pauseEvent += OnGamePauseChange;
    }

    private void OnGamePauseChange(bool isPaused)
    {
        if (isPaused)
            audioSource.Pause();
        else
            audioSource.UnPause();
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
        {
            entityPlayer.isCharging = true;
            audioSource.volume = audioSource.pitch = 1.0f;
            audioSource.Play();
            charging = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
        {
            entityPlayer.energy += chargeSpeed * Time.deltaTime;
            if (entityPlayer.energy >= 1.0f)
                charging = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EntityPlayer entityPlayer = other.GetComponent<EntityPlayer>();
        if (entityPlayer != null)
        {
            entityPlayer.isCharging = false;
            charging = false;
        }
    }

    private void Update()
    {
        if(!charging && audioSource.volume > 0)
        {
            audioSource.volume = audioSource.pitch = Mathf.MoveTowards(audioSource.volume, 0, audioDecreaseSpeed * Time.unscaledDeltaTime);
            if (audioSource.volume == 0)
                audioSource.Stop();
        }
    }
}