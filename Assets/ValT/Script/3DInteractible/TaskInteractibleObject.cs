using UnityEngine;

public class TaskInteractibleObject : InteractableObject
{
    [Header("Task Menu")]
    [SerializeField] private GameObject notIntroMenuPrefab;
    [SerializeField] private GameObject interactionMenuPrefab;


    public override bool Interact()
    {
        //Gérer la partie task
        Instantiate(interactionMenuPrefab).GetComponent<GeneralTaskMenu>().ConfigureMenu(isIntro ? 0 : taskPoints, successDialogueId, interactSequenceOrder);
        
        //On active la base
        base.Interact();

        //On ne peut plus bouger après avoir interagi avec l'objet
        return true;
    }

    public override void TurnIntoNotIntroObject()
    {
        base.TurnIntoNotIntroObject();
        interactionMenuPrefab = notIntroMenuPrefab;
    }

}
