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
    private bool diveAway = false;

    [Header("Teleportation")]
    public float teleportMinInterval = 5, teleportMaxInterval = 15;
    public float teleportTauntChance = 0.05f;
    public float teleportMinDistance = 3.0f, teleportMaxDistance = 8.0f;
    public LayerMask teleportCheckMask;
    private float teleportTimer;

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
        playerHead = player.head;
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.GamePaused)
            return;

        PlayIdleSFX();
        SeekPlayer();
        MoveHead();
        DiveAway();

        if (state == State.Active)
        {
            if (!WithinView(playerHead, transform, player.viewAngle, player.viewRange, teleportCheckMask))
                Teleport();
            else
            {
                if (player.flashlightState && WithinView(playerHead, transform, player.flashlightDetectAngle, player.flashlightDetectRange, teleportCheckMask))
                {
                    state = State.Fleeing;
                    diveAway = true;
                    activeSfxAudio.Stop();
                    fleeSfxAudio.PlayRandom();
                    Destroy(gameObject, 5.0f);
                }
                else if (!diveAway)
                {
                    diveAway = true;
                    activeSfxAudio.PlayRandom();
                }
            }
        }
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

    private void Teleport()
    {
        teleportTimer -= Time.deltaTime;
        if(teleportTimer < 0)
        {
            teleportTimer = Random.Range(teleportMinInterval, teleportMaxInterval);
            bool foundSpot = false;
            Vector3 newSpot = new Vector3();
            int attempts = 0;
            while(!foundSpot && attempts < 60)
            {
                newSpot = (Quaternion.Euler(0, Random.value * 360, 0) * Vector3.forward) * Random.Range(teleportMinDistance, teleportMaxDistance);
                newSpot += player.transform.position;

                Vector3 direction = newSpot - playerHead.position;
                if(!Physics.Raycast(playerHead.position, direction.normalized, direction.magnitude, teleportCheckMask))
                    foundSpot = !WithinView(playerHead, newSpot, player.viewAngle, player.viewRange, teleportCheckMask);

                attempts++;
            }

            if (foundSpot)
            {
                diveAway = false;
                transform.position = newSpot;
                if (Random.value < teleportTauntChance)
                    activeSfxAudio.PlayRandom();
            }
        }
    }

    private void DiveAway()
    {
        if(diveAway)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -2.0f, transform.position.z), Time.deltaTime);
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