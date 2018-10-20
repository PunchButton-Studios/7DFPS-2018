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

    [Header("Movement")]
    public float movementSpeed = 10.0f;
    public float lookSpeed = 1.0f;

    private float lookXAngle = 0.0f;
    public float lookMinAngle = -85.0f, lookMaxAngle = 85.0f;

    private void Reset()
    {
        this.controller = GetComponent<CharacterController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    #region Input & Movement
    private void HandleInput()
    {
        if (GameManager.GamePaused)
            return;
        Move();
        Look();
    }

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
    #endregion
}