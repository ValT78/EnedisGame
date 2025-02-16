using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool resetAfterIntro;
    [SerializeField] protected bool isIntro;
    //N'apparait dans Unity que si isIntro est à true
    [SerializeField] protected int interactSequenceOrder;
    [SerializeField] protected int taskPoints;
    [SerializeField] protected string notIntrosuccessDialogueId;
    [SerializeField] protected string successDialogueId;



    [Header("Visual Settings")]
    [SerializeField] protected ParticleSystem interactionParticles;
    public Transform pressKeyCanvasPosition;
    [SerializeField] private Renderer objectRenderer;

    private Material originalMaterial;
    private Material highlightMaterial;
    private Coroutine highlightCoroutine;

    protected bool alreadyInteracted = false;


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

    public virtual bool Interact()
    {
        interactionParticles.Play();
        alreadyInteracted = true;
        return false;
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

    public bool CanInteractWith()
    {
        if(!alreadyInteracted && isIntro == GameManager.isIntro && (!isIntro || GameManager.lastInteractedObject == interactSequenceOrder - 1)) return true;
        return false;
    }

    public virtual void TurnIntoNotIntroObject()
    {
        alreadyInteracted = false;
        isIntro = false;
        if(notIntrosuccessDialogueId != "") successDialogueId = notIntrosuccessDialogueId;
    }

    public bool GetIsIntro()
    {
        return isIntro;
    }
    
    public bool GetResetAfterIntro()
    {
        return resetAfterIntro;
    }

}
