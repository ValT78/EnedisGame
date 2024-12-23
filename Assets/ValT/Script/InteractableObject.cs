using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public Canvas interactionCanvas;
    public Transform canvasPosition;
    public ParticleSystem interactionParticles;
    public GameObject interactionMenuPrefab;

    private Camera playerCamera;
    private bool isPlayerLooking;
    private bool isPlayerClose;
    private bool hasOpenedMenu = false;

    private GameObject activeMenu;

    private void Start()
    {
        playerCamera = Camera.main;
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false); // Hide the canvas initially
        }
    }

    private void Update()
    {
        CheckPlayerInteraction();

        if (isPlayerLooking && isPlayerClose && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void CheckPlayerInteraction()
    {
        // Check distance between the player and the object
        isPlayerClose = Vector3.Distance(playerCamera.transform.position, transform.position) <= interactionRange;

        if (isPlayerClose)
        {
            // Check if the player is looking at the object
            Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
            {
                isPlayerLooking = hit.transform == transform;
            }
            else
            {
                isPlayerLooking = false;
            }
        }
        else
        {
            isPlayerLooking = false;
        }

        // Show or hide the interaction canvas based on conditions
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(isPlayerLooking && isPlayerClose);

            // Optional: Align the canvas to face the player
            if (interactionCanvas.gameObject.activeSelf)
            {
                interactionCanvas.transform.position = canvasPosition != null ? canvasPosition.position : transform.position;
                interactionCanvas.transform.LookAt(playerCamera.transform);
                interactionCanvas.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
            }
        }
    }

    private void Interact()
    {
        // Trigger particle effect
        if (interactionParticles != null)
        {
            interactionParticles.Play();
        }

        // Open menu if it's the first interaction
        if (!hasOpenedMenu && interactionMenuPrefab != null)
        {
            hasOpenedMenu = true;
            activeMenu = Instantiate(interactionMenuPrefab);

            // Position the menu in front of the player
            activeMenu.transform.SetParent(playerCamera.transform, false);
            activeMenu.transform.localPosition = new Vector3(0f, 0f, 2f);
            activeMenu.transform.localRotation = Quaternion.identity;

            // Add functionality to the close button
            Button closeButton = activeMenu.GetComponentInChildren<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseMenu);
            }
        }
    }

    private void CloseMenu()
    {
        if (activeMenu != null)
        {
            Destroy(activeMenu);
            activeMenu = null;
        }
    }
}
