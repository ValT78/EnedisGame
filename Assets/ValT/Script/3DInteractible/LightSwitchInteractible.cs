using UnityEngine;

public class LightSwitchInteractible : InteractableObject
{
    [Header("Light Settings")]
    [SerializeField] private Light targetLight; // Lumière à activer/désactiver

    public override bool Interact()
    {
        targetLight.enabled = !targetLight.enabled;

        //Gérer la partie task
        GameManager.Instance.CompleteTask(isIntro ? 0 : taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        GameManager.lastInteractedObject = interactSequenceOrder;

        //On active la base
        base.Interact();

        //On peut toujours bouger après avoir interagi avec l'objet²
        return false;
    }
}
