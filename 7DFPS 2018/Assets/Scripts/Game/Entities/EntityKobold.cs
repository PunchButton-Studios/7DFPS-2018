using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityKobold : Entity
{
    public State state = State.Idle;

    private EntityPlayer player;

    [Header("Head Movement")]
    public Transform head;
    public Transform playerHead;
    public float headMovementSpeed = 5.0f;

    [Header("AI")]
    public float playerDetectionRange = 20;
    public Vector3 playerDetectionOffset = new Vector3(0, 1, 15);

    [Header("SFX")]
    public AudioController idleSfxAudio;
    public AudioController activeSfxAudio, fleeSfxAudio;
    public float idleSfxMinInterval = 30, idleSfxMaxInterval = 90;
    private float nextIdleSfx;

    protected override void Start()
    {
        base.Start();
        nextIdleSfx = Time.time + Random.Range(idleSfxMinInterval, idleSfxMaxInterval);
        player = FindObjectOfType<EntityPlayer>();
    }

    protected override void Update()
    {
        base.Update();
        PlayIdleSFX();
        SeekPlayer();
        MoveHead();
    }

    private void SeekPlayer()
    {
        if(state == State.Idle)
        {
            Vector3 checkCenter = head.position + transform.rotation * playerDetectionOffset;
            if (Vector3.Distance(player.transform.position, checkCenter) <= playerDetectionRange)
                state = State.Active;
        }
    }

    private void MoveHead()
    {
        if (state == State.Active)
        {
            Vector3 direction = playerHead.position - head.position;
            head.rotation = Quaternion.RotateTowards(head.rotation, Quaternion.LookRotation(direction, Vector3.up), headMovementSpeed * Time.deltaTime);
        }
    }

    private void PlayIdleSFX()
    {
        if(Time.time > nextIdleSfx)
        {
            if (state == State.Idle)
                idleSfxAudio.PlayRandom();
            else if (state == State.Active)
                activeSfxAudio.PlayRandom();
            nextIdleSfx = Time.time + Random.Range(idleSfxMinInterval, idleSfxMaxInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(head.position + transform.rotation * playerDetectionOffset, playerDetectionRange);
    }

    public enum State
    {
        Idle,
        Active,
        Startled,
        Fleeing,
    }
}