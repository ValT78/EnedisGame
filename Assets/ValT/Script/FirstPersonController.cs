using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonController : MonoBehaviour
{
    static public FirstPersonController Instance;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float sprintMultiplier;
    public float jumpForce;
    public float gravity = -9.81f;
    public float smoothAccelerationTime;

    [Header("Camera Settings")]
    public float mouseSensitivity;
    public Transform cameraTransform;
    public float verticalLookLimit;

    [Header("Interaction Settings")]
    public float interactionRange;
    public LayerMask interactableLayer;
    public Canvas interactionCanvas;
    public float canvasFadeDuration;
    public float highlightFadeDuration;
    public float hoverFloatSpeed;
    public float hoverFloatAmplitude;

    // Movement variables 
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private Vector3 currentVelocity;
    private Vector3 smoothVelocity;

    // Camera variables
    private float xRotation = 0f;

    // Input system variables and references 
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    // Interaction variables
    private Camera playerCamera;
    private InteractableObject currentInteractable;
    private CanvasGroup canvasGroup;
    private Coroutine canvasFadeCoroutine;
    private Vector3 lastCanvasPosition;
    private bool isDoingTask = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        interactAction = playerInput.actions["Interact"];

        if (interactionCanvas != null)
        {
            if (!interactionCanvas.TryGetComponent(out canvasGroup))
            {
                canvasGroup = interactionCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
        }
    }

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (!isDoingTask && !GameManager.Instance.isPaused)
        {
            HandleMovement();
            HandleMouseLook();
            DetectInteractable();
        }

        if (currentInteractable != null && interactAction.WasPressedThisFrame())
        {
            isDoingTask = currentInteractable.Interact();
        }

        UpdateCanvasPosition();

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
            if (jumpAction.IsPressed())
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

    private void DetectInteractable()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != currentInteractable)
            {
                if (canvasFadeCoroutine != null)
                {
                    StopCoroutine(canvasFadeCoroutine);
                }

                if (currentInteractable != null)
                {
                    currentInteractable.DisableHighlight(highlightFadeDuration);
                }

                currentInteractable = interactable;

                if (currentInteractable != null)
                {
                    currentInteractable.EnableHighlight(highlightFadeDuration);
                    if (interactionCanvas != null)
                    {
                        canvasFadeCoroutine = StartCoroutine(FadeCanvas(1f));
                    }
                }
                else if (interactionCanvas != null)
                {
                    canvasFadeCoroutine = StartCoroutine(FadeCanvas(0f));
                }
            }
        }
        else if (currentInteractable != null)
        {
            if (canvasFadeCoroutine != null)
            {
                StopCoroutine(canvasFadeCoroutine);
            }

            currentInteractable.DisableHighlight(highlightFadeDuration);
            currentInteractable = null;

            if (interactionCanvas != null)
            {
                canvasFadeCoroutine = StartCoroutine(FadeCanvas(0f));
            }
        }
    }

    private void UpdateCanvasPosition()
    {
        if (currentInteractable != null && canvasGroup.alpha > 0)
        {
            lastCanvasPosition = currentInteractable.canvasPosition.position;

            if (lastCanvasPosition != null)
            {
                interactionCanvas.transform.position = lastCanvasPosition;
            }

            interactionCanvas.transform.LookAt(playerCamera.transform);
            interactionCanvas.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);

            // Add floating effect
            interactionCanvas.transform.position += Vector3.up * Mathf.Sin(Time.time * hoverFloatSpeed) * hoverFloatAmplitude;
        }
        else if (canvasGroup.alpha > 0)
        {
            interactionCanvas.transform.position = lastCanvasPosition;
            interactionCanvas.transform.LookAt(playerCamera.transform);
            interactionCanvas.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
            interactionCanvas.transform.position += hoverFloatAmplitude * Mathf.Sin(Time.time * hoverFloatSpeed) * Vector3.up;

        }
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < canvasFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / canvasFadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void CompleteTask()
    {
        isDoingTask = false;
    }
}
