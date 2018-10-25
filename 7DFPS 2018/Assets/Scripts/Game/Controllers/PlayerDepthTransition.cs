using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDepthTransition : MonoBehaviour
{
    public EntityPlayer entityPlayer;
    public CharacterController characterController;
    public Transform playerHead, worldHolder, playerBase;
    public ChunkHandler chunkHandler;
    public WorldGenerator worldGenerator;
    public bool active = false;

    public Animator animator;
    public string transferTrigger, endTrigger;
    public AudioController impactSfx, jointSfx, fallSfx;
    public AudioSource cameraRotateSfx;
    public CameraController cameraController;
    public Transform tube;
    public Vector3 tubeHoleOffset;

    private Vector3 defaultPosition;
    public float rotationXTarget = 0.0f;
    public float rotationXSpeed = 25.0f;
    public float forwardOffset = 0.0f;
    public float yOffset = 0.0f;

    public float yOffsetWorldOnWorldgen = 100.0f;

    private Vector2Int wgStartPos;

    private void Reset()
    {
        entityPlayer = GetComponent<EntityPlayer>();
        characterController = GetComponent<CharacterController>();
    }

    public void TransferDown(Vector2Int tilePosition, Vector3 holeCenter)
    {
        wgStartPos = tilePosition;
        entityPlayer.enabled = false;
        characterController.enabled = false;
        entityPlayer.ResetCameraXAngle();
        defaultPosition = transform.position;
        animator.SetTrigger(transferTrigger);
        cameraRotateSfx.volume = 0.0f;

        tube.position = holeCenter + tubeHoleOffset;
        active = true;
    }

    public void SetupWorldgen()
    {
        transform.Translate(0, yOffsetWorldOnWorldgen, 0);
        defaultPosition.y += yOffsetWorldOnWorldgen;
        tube.Translate(0, yOffsetWorldOnWorldgen, 0);
        tube.parent = transform;
        tube.GetComponent<TunnelDropEffect>().enabled = true;
        worldHolder.Translate(0, yOffsetWorldOnWorldgen, 0);
        playerBase.Translate(0, yOffsetWorldOnWorldgen, 0);
        chunkHandler.DestroyChildren();

        worldGenerator.worldGenCompleteEvent += OnWorldGenComplete;
        worldGenerator.GenerateLevel(100, wgStartPos);
    }

    private void OnWorldGenComplete()
    {
        worldGenerator.worldGenCompleteEvent -= OnWorldGenComplete;
        animator.SetTrigger(endTrigger);
    }

    public void DetachTube()
    {
        tube.parent = null;
        tube.transform.position = new Vector3(0, 1000, 0);
        tube.GetComponent<TunnelDropEffect>().enabled = false;
    }

    public void EndTransfer()
    {
        active = false;
        characterController.enabled = true;
        entityPlayer.enabled = true;
    }

    private void Update()
    {
        if (!active)
            return;

        playerHead.rotation = Quaternion.RotateTowards(
            playerHead.rotation,
            Quaternion.Euler(rotationXTarget, playerHead.rotation.eulerAngles.y, 0),
            rotationXSpeed * Time.deltaTime
            );

        transform.position = defaultPosition + transform.forward * forwardOffset + Vector3.up * yOffset;
    }

    public void PlayImpactSFX()
    {
        impactSfx.PlayRandom();
        cameraController.Bob();
    }
    public void PlayJointSFX() => jointSfx.PlayNext();
    public void PlayFallSFX() => fallSfx.PlayRandom();
}