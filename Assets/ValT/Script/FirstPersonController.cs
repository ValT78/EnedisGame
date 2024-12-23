using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float smoothAccelerationTime = 0.1f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    public float verticalLookLimit = 85f;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private Vector3 currentVelocity;
    private Vector3 smoothVelocity;

    private float xRotation = 0f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // Get input
        moveInput = moveAction.ReadValue<Vector2>();

        // Calculate movement direction
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Apply sprint multiplier
        float currentSpeed = moveSpeed * (sprintAction.IsPressed() ? sprintMultiplier : 1f);

        // Smooth movement
        Vector3 targetVelocity = moveDirection * currentSpeed;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref smoothVelocity, smoothAccelerationTime);

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f; // Small constant to keep grounded
            if (jumpAction.triggered)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Final movement
        Vector3 velocity = currentVelocity + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Get input
        lookInput = lookAction.ReadValue<Vector2>();

        // Calculate rotation
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }
}
