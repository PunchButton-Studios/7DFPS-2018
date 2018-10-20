using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EntityPlayer : Entity
{
    public CharacterController controller;
    public Transform head;

    [Header("Input")]
    public string horizontalMovementAxis = "Horizontal";
    public string verticalMovementAxis = "Vertical";
    public string horizontalLookAxis = "Mouse X";
    public string verticalLookAxis = "Mouse Y";
    public string actionButton = "Action";

    [Header("Movement")]
    public float movementSpeed = 10.0f;
    public float lookSpeed = 1.0f;

    private float lookXAngle = 0.0f;
    public float lookMinAngle = -85.0f, lookMaxAngle = 85.0f;

    private float fallSpeed = 0.0f;
    public float gravity = 9.8f;
    public float maxFallSpeed = 20.0f;

    [Header("Action")]
    public float maxActivateRange = 2.5f;
    public PlayerGUI gui;

    [Header("Anxiety")]
    public float anxiety = 0.0f;
    public float maxAnxiety = 128.0f;
    public float darknessAnxiety = 1.0f;
    public LayerMask lampMask;
    public float lightCheckRange = 16.0f;

    private void Reset()
    {
        this.controller = GetComponent<CharacterController>();
        this.gui = GetComponentInChildren<PlayerGUI>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        Activatable activatable = GetActivatable();
        HandleInput(activatable);
        HandleAnxiety();
        UpdateGUI(activatable);
    }

    private void HandleInput(Activatable activatable)
    {
        if (GameManager.GamePaused)
            return;
        Move();
        Look();
        Fall();
        Action(activatable);
    }

    private void UpdateGUI(Activatable activatable) => gui.UpdateGUI(activatable, this);

    #region Movement
    private void Move()
    {
        float horizontal = Input.GetAxisRaw(horizontalMovementAxis);
        float vertical = Input.GetAxisRaw(verticalMovementAxis);

        Vector3 strafeMovement = transform.right * horizontal;
        Vector3 forwardMovement = transform.forward * vertical;

        Vector3 normalizedMovementDirection = (strafeMovement + forwardMovement).normalized;
        controller.Move(normalizedMovementDirection * movementSpeed * Time.deltaTime);
    }

    private void Look()
    {
        //Note: Don't Time.deltaTime mouse movement. It'll slow down mouse movement the higher the fps is.
        float mouseX = Input.GetAxisRaw(horizontalLookAxis);
        float mouseY = Input.GetAxisRaw(verticalLookAxis);

        transform.Rotate(new Vector3(0, mouseX * lookSpeed, 0)); //Rotate Left/Right

        //Up/Down rotation
        lookXAngle -= mouseY * lookSpeed;
        lookXAngle = Mathf.Clamp(lookXAngle, lookMinAngle, lookMaxAngle);
        head.transform.localRotation = Quaternion.Euler(lookXAngle, 0, 0);
    }

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
        if (Physics.Raycast(head.position, head.forward, out hit, maxActivateRange))
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
        if (Input.GetButton(actionButton))
            activatable?.Activate(this);
    }
    #endregion

    private void HandleAnxiety()
    {
        float modifier = 1.0f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, lightCheckRange, lampMask);
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
            }
        }

        if (modifier > 0.0f)
            anxiety += darknessAnxiety * modifier * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(head.position, head.forward * maxActivateRange); //Activation range
    }
}