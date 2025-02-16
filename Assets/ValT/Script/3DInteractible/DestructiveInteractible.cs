using UnityEngine;

public class DestructiveInteractible : InteractableObject
{

    public override bool Interact()
    {
        GameManager.Instance.CompleteTask(isIntro ? 0 : taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        GameManager.lastInteractedObject = interactSequenceOrder;

        base.Interact();

        //On détruit l'objet juste après avoir return false
        Destroy(gameObject, 0.1f);

        //On ne peut plus bouger après avoir interagi avec l'objet
        return false;
    }

    public override void TurnIntoNotIntroObject()
    {
        base.TurnIntoNotIntroObject();
    }

}
