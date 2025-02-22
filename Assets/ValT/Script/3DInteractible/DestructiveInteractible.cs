using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructiveInteractible : InteractableObject
{
    [SerializeField] private GameObject model;
    [SerializeField] private float timebeforeDestroy;

    [SerializeField] private UnityEvent beforeDestroy; // Événement déclenché


    public override bool Interact()
    {
        if(model != null) model.SetActive(false);

        GameManager.Instance.CompleteTask(isIntro ? 0 : taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        GameManager.lastInteractedObject = interactSequenceOrder;

        base.Interact();

        //On détruit l'objet juste après avoir return false
        StartCoroutine(DestroyObject());

        //On ne peut plus bouger après avoir interagi avec l'objet
        return true;
    }

    public override void TurnIntoNotIntroObject()
    {
        base.TurnIntoNotIntroObject();
    }

    private IEnumerator DestroyObject()
    {
        //Il faut attendre au moins 0.1f pour que l'objet soit détruit, le temps que l'autre fonction retourne false
        yield return new WaitForSeconds(timebeforeDestroy==0 ? 0.1f : timebeforeDestroy);
        beforeDestroy.Invoke();
        Destroy(model);
    }

}
