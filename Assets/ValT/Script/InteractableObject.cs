using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public ParticleSystem interactionParticles;
    public GameObject interactionMenuPrefab;
    public Transform canvasPosition;
    public Renderer objectRenderer;
    public bool isBlockingPlayer;

    private bool hasOpenedMenu = false;
    private Material originalMaterial;
    private Material highlightMaterial;
    private Coroutine highlightCoroutine;


    private void Start()
    {
        if (objectRenderer != null)
        {
            originalMaterial = new Material(objectRenderer.material);
            highlightMaterial = new Material(originalMaterial);
            highlightMaterial.SetFloat("_Glossiness", 1f);
            highlightMaterial.color = Color.cyan;
            highlightMaterial.SetColor("_EmissionColor", Color.cyan * 2f);
        }
    }

    public bool Interact()
    {
        interactionParticles.Play();
        // Open menu if it's the first interaction
        if (!hasOpenedMenu)
        {
            hasOpenedMenu = true;
            Instantiate(interactionMenuPrefab);
            return isBlockingPlayer;
        }
        else
        {
            return false;
        }
    }

    public void EnableHighlight(float duration)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(FadeHighlight(true, duration));
    }

    public void DisableHighlight(float duration)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(FadeHighlight(false, duration));
    }

    private IEnumerator FadeHighlight(bool enable, float duration)
    {
        float startLerp = enable ? 0f : 1f;
        float endLerp = enable ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Lerp(startLerp, endLerp, elapsedTime / duration);
            objectRenderer.material.Lerp(originalMaterial, highlightMaterial, lerpFactor);
            yield return null;
        }
        objectRenderer.material = enable ? highlightMaterial : originalMaterial;
    }

}
