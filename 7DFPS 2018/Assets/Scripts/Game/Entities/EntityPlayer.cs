using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EntityPlayer : Entity
{
    public CharacterController controller;
    public Transform head, playerBase;

    [Header("Input")]
    public string horizontalLookAxis = "Mouse X";
    public string verticalLookAxis = "Mouse Y";

    [Header("Movement")]
    public float movementSpeed = 10.0f;
    public float lookSpeed = 1.0f;
    
    private float lookXAngle = 0.0f;
    public float lookMinAngle = -85.0f, lookMaxAngle = 85.0f;

    private float fallSpeed = 0.0f;
    public float gravity = 9.8f;
    public float maxFallSpeed = 20.0f;
    
    private bool isMoving = false;
    private float rotationChange = 0.0f;

    [Header("Action")]
    public float maxActivateRange = 2.5f;
    public float activationTimer = 0.0f;
    public LayerMask activationMask;
    public PlayerGUI gui;

    [Header("Anxiety")]
    public float anxiety = 0.0f;
    public float maxAnxiety = 128.0f;
    public float darknessAnxiety = 1.0f;
    private KoboldSpawner koboldSpawner;
    public float anxietyPerKobold = 0.8f;
    public float flashlightAnxietyModifier = 0.3f;
    public LayerMask anxietyMask;
    public float lightCheckRange = 16.0f;

    [Header("Time Limit")]
    public float timeLimit = 1800.0f;
    public float maxTimeLimit = 1800.0f;
    public float anxietyTimeDecrease = 2.0f;

    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightToggleSpeed = 0.3f;
    public float flashlightIntensity = 600.0f;
    public float flashlightDetectRange = 20.0f, flashlightDetectAngle = 15.0f;
    private float flashlightTransition = 0.0f;
    public bool flashlightState = false;

    [Header("Energy")]
    [Range(0,1)] public float energy = 1.0f;
    public bool isCharging = false;
    public float flashlightEnergyCost = 0.001f;
    public float passiveEnergyCost = 0.0001f;

    [Header("Home Calling")]
    public Vector3 homeCallFreeSpaceRange;
    public LayerMask homeCallCollisionMask;
    public float homeCallTimer = 0.0f;
    public float homeCallTimerIdleDecrease = 2.0f;
    public AnimationCurve homeCallTimePerDistance;
    public float homeCallTimeMultiplier = 0.25f;

    [Header("View")]
    public float viewRange = 50.0f;
    public float viewAngle = 60.0f;

    [Header("Effects")]
    public CameraController cameraController;
    public AudioController jointAudio, impactAudio, flashlightAudio, batteryAudio;
    public AudioSource rotateAudio, criticalAnxietySfx;
    public MusicHandler heartbeatSfx;
    public float jointVolumeIncreaseSpeed = 0.1f, jointVolumeDecreaseSpeed = 0.25f;
    public float rotateVolumeEffect = 1.0f, rotatePitchEffect = 1.0f;
    public float rotateMaxVolume = 1.0f, rotateMaxPitch = 2.0f, rotateMinPitch = 0.5f;
    public float rotateVolumeLerpSpeed = 0.8f, rotatePitchLerpSpeed = 0.8f;
    public float anxietyCriticalSpeed = 0.1f;
    [Range(0, 1)] public float anxietyCriticalPoint = 0.9f, anxietyCriticalMaxVolume = 0.5f;
    public AnimationCurve anxietyFieldOfViewCurve;
    public AnimationCurve anxietyCameraShakeCurve;

    private bool playedBatteryAlarm = false;

    private float bobTimer = 0.0f;
    public float bobInterval = 0.57f;
    public float bobInit = 0.31f;
    public float stepDelay = 0.23f;
    private List<float> stepTimes = new List<float>();

    public float HomeCallTime
    {
        get
        {
            return homeCallTimePerDistance.Evaluate(Vector3.Distance(transform.position, playerBase.position)) * homeCallTimeMultiplier;
        }
    }

    private void Reset()
    {
        this.controller = GetComponent<CharacterController>();
        this.gui = GetComponentInChildren<PlayerGUI>();
    }

    protected override void Awake()
    {
        timeLimit = maxTimeLimit;
        GameManager.Main.loadGameEvent += OnGameLoaded;
        GameManager.Main.saveGameEvent += OnGameSave;
    }

    private void OnGameSave(SaveData saveData)
    {
        saveData.playerData.anxiety = this.anxiety;
        saveData.playerData.energy = this.energy;
        saveData.playerData.position = transform.position;
        saveData.playerData.rotation = Quaternion.Euler(lookXAngle, transform.rotation.eulerAngles.y, 0.0f);
        saveData.playerData.timeLeft = timeLimit / maxTimeLimit;
        saveData.playerData.flashlight = flashlightState;
    }

    private void OnGameLoaded(SaveData saveData)
    {
        this.anxiety = saveData.playerData.anxiety;
        this.energy = saveData.playerData.energy;
        transform.position = saveData.playerData.position;
        transform.rotation = Quaternion.Euler(0.0f, saveData.playerData.rotation.eulerAngles.y, 0.0f);
        lookXAngle = saveData.playerData.rotation.eulerAngles.x;
        timeLimit = maxTimeLimit * saveData.playerData.timeLeft;
        flashlightState = saveData.playerData.flashlight;
    }

    protected override void Start()
    {
        base.Start();
        bobTimer = bobInit;
        koboldSpawner = FindObjectOfType<KoboldSpawner>();
    }

    protected override void Update()
    {
        base.Update();
        Activatable activatable = GetActivatable();
        bool canCallHome = CanCallHome();
        HandleInput(activatable, canCallHome);
        HandleAnxiety();
        DecreaseTimeLimit();
        HandleFlashLight();
        DecreaseEnergy();
        PerformEffects();
        UpdateGUI(activatable, canCallHome);
    }

    private void HandleInput(Activatable activatable, bool canCallHome)
    {
        if (GameManager.GamePaused)
            return;
        Look();
        Fall();

        if (!CallHome(canCallHome))
        {
            Move();
            Action(activatable);
            ToggleFlashlight();
        }
        else
            isMoving = false;
    }

    private void UpdateGUI(Activatable activatable, bool canCallHome) => gui.UpdateGUI(activatable, canCallHome, this);

    #region Movement
    private void Move()
    {
        float horizontal = InputHandler.GetAxis(InputHandler.Input.MovementHorizontal);
        float vertical = InputHandler.GetAxis(InputHandler.Input.MovementVertical);

        Vector3 strafeMovement = transform.right * horizontal;
        Vector3 forwardMovement = transform.forward * vertical;

        Vector3 normalizedMovementDirection = (strafeMovement + forwardMovement).normalized;
        controller.Move(normalizedMovementDirection * movementSpeed * Time.deltaTime);

        isMoving = normalizedMovementDirection.magnitude > 0.0f;
    }

    private void Look()
    {
        //Note: Don't Time.deltaTime mouse movement. It'll slow down mouse movement the higher the fps is.
        float mouseX = Input.GetAxisRaw(horizontalLookAxis);
        float mouseY = Input.GetAxisRaw(verticalLookAxis);

        transform.Rotate(new Vector3(0, mouseX * lookSpeed, 0)); //Rotate Left/Right

        //Up/Down rotation
        float oldLookXAngle = lookXAngle;
        lookXAngle -= mouseY * lookSpeed;
        lookXAngle = Mathf.Clamp(lookXAngle, lookMinAngle, lookMaxAngle);
        head.transform.localRotation = Quaternion.Euler(lookXAngle, 0, 0);

        rotationChange = Mathf.Abs(mouseX) + Mathf.Abs(lookXAngle - oldLookXAngle);
    }

    public void ResetCameraXAngle() => lookXAngle = 0;

    private void Fall()
    {
        if (controller.isGrounded)
            fallSpeed = 0.0f;
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            if (fallSpeed > maxFallSpeed)
                fallSpeed = maxFallSpeed;
            controller.Move(new Vector3(0, -fallSpeed, 0));
        }
    }
    #endregion
    #region Action
    private Activatable GetActivatable()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, maxActivateRange, activationMask))
        {
            Activatable activatable = hit.collider.GetComponent<Activatable>();
            if (activatable != null && activatable.CanBeActivated(this))
                return activatable;
            else
                return null;
        }
        else
            return null;
    }

    private void Action(Activatable activatable)
    {
        if (InputHandler.GetButton(InputHandler.Input.Action) && activatable != null)
        {
            activationTimer += Time.deltaTime;
            if (activationTimer > activatable.ActivationTime)
            {
                activatable.Activate(this);
                activationTimer = 0.0f;
            }
        }
        else
            activationTimer = 0.0f;
    }
    #endregion

    private bool CanCallHome() => !Physics.CheckBox(transform.position, homeCallFreeSpaceRange, Quaternion.identity, homeCallCollisionMask);

    private bool CallHome(bool canCallHome)
    {
        if (InputHandler.GetButton(InputHandler.Input.CallHome) && canCallHome)
        {
            homeCallTimer += Time.deltaTime;

            if(homeCallTimer > HomeCallTime)
            {
                playerBase.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
                BaseController.Main.collectingState = BaseController.CollectingState.CollectingTaggedOres;
                homeCallTimer = 0.0f;
            }

            return true;
        }
        else
        {
            homeCallTimer -= homeCallTimerIdleDecrease * Time.deltaTime;
            if (homeCallTimer < 0.0f)
                homeCallTimer = 0.0f;
            return false;
        }
    }

    private void ToggleFlashlight()
    {
        if (InputHandler.GetButtonDown(InputHandler.Input.Flashlight) && energy > 0.0f)
        {
            flashlightState = !flashlightState;
            flashlightAudio.Play(flashlightState ? 0 : 1);
        }
    }

    private void HandleFlashLight()
    {
        if(energy <= 0.0f && flashlightState)
        {
            flashlightState = false;
            flashlightAudio.Play(1);
        }

        flashlightTransition = Mathf.MoveTowards(flashlightTransition, flashlightState ? 1 : 0, flashlightToggleSpeed * Time.deltaTime);

        if (flashlightTransition > 0.0f)
        {
            flashlight.enabled = true;
            flashlight.intensity = flashlightTransition * flashlightIntensity;
        }
        else
            flashlight.enabled = false;
    }

    private void DecreaseTimeLimit()
    {
        if (GameManager.GamePaused)
            return; //Just don't.

        timeLimit -= (1 + (anxiety / maxAnxiety) * anxietyTimeDecrease) * Time.deltaTime;
    }

    private void DecreaseEnergy()
    {
        energy -= (passiveEnergyCost + (flashlightEnergyCost * flashlightTransition)) * Time.deltaTime;
        energy = Mathf.Clamp01(energy);

        if (!playedBatteryAlarm && energy < 0.3333f)
        {
            batteryAudio.PlayRandom();
            playedBatteryAlarm = true;
        }
        else if (playedBatteryAlarm && energy > 0.3333f)
            playedBatteryAlarm = false;
    }

    private void HandleAnxiety()
    {
        float modifier = 1.0f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, lightCheckRange, anxietyMask);
        if(colliders.Length > 0)
        {
            foreach(Collider c in colliders)
            {
                ILightSource lightSource = c.GetComponent<ILightSource>();
                if(lightSource != null)
                {
                    if(Vector3.Distance(transform.position, c.transform.position) < lightSource.Range)
                        modifier -= lightSource.Power;
                }

                IRelaxationObject relaxationObject = c.GetComponent<IRelaxationObject>();
                if(relaxationObject != null)
                    anxiety -= relaxationObject.AnxietyDecreaseAmount * Time.deltaTime;
            }
        }

        modifier *= 1 - (flashlightTransition * (1 - flashlightAnxietyModifier));

        if (modifier > 0.0f)
            anxiety += (darknessAnxiety * modifier + koboldSpawner.KoboldCount * anxietyPerKobold) * Time.deltaTime;

        anxiety = Mathf.Clamp(anxiety, 0, maxAnxiety);
    }

    private void PerformEffects()
    {
        if (isMoving)
        {
            bobTimer += Time.deltaTime;

            if(bobTimer > bobInterval)
            {
                impactAudio.PlayRandom();
                stepTimes.Add(Time.time + stepDelay);
                bobTimer = 0.0f;
                cameraController.Bob();
            }
        }

        if(stepTimes.Count > 0 && Time.time > stepTimes[0])
        {
            jointAudio.PlayNext();
            stepTimes.RemoveAt(0);
        }
        
        jointAudio.Volume = Mathf.MoveTowards(jointAudio.Volume, isMoving ? 1 : 0, (isMoving ? jointVolumeIncreaseSpeed : jointVolumeDecreaseSpeed));

        if (GameManager.GamePaused)
            rotationChange = 0.0f;
        rotateAudio.volume = Mathf.Clamp(Mathf.Lerp(rotateAudio.volume, rotationChange / rotateVolumeEffect, rotateVolumeLerpSpeed), 0, rotateMaxVolume);
        rotateAudio.pitch = Mathf.Clamp(Mathf.Lerp(rotateAudio.pitch, rotationChange / rotatePitchEffect + rotateMinPitch, rotatePitchLerpSpeed), rotateMinPitch, rotateMaxPitch);

        float anxietyPercentage = anxiety / maxAnxiety;
        heartbeatSfx.Play((int)Mathf.Floor(anxietyPercentage * 4));
        criticalAnxietySfx.volume = Mathf.MoveTowards(criticalAnxietySfx.volume, anxietyPercentage > anxietyCriticalPoint ? anxietyCriticalMaxVolume : 0, anxietyCriticalSpeed * Time.deltaTime);
        cameraController.FOV = anxietyFieldOfViewCurve.Evaluate(anxietyPercentage);
        cameraController.Shake(anxietyCameraShakeCurve.Evaluate(anxietyPercentage));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(head.position, head.forward * maxActivateRange); //Activation range

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, homeCallFreeSpaceRange * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * flashlightDetectRange);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, flashlightDetectAngle, 0) * transform.forward).normalized * flashlightDetectRange);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, -flashlightDetectAngle, 0) * transform.forward).normalized * flashlightDetectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * viewRange);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, viewAngle, 0) * transform.forward).normalized * viewRange);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, -viewAngle, 0) * transform.forward).normalized * viewRange);
    }
}