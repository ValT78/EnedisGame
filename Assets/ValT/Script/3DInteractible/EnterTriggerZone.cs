using UnityEngine;

public class EnterTriggerZone : MonoBehaviour
{
    [SerializeField] private int interactSequenceOrder;
    [SerializeField] private string dialogueId;
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.lastInteractedObject == interactSequenceOrder-1 && TryGetComponent<FirstPersonController>(out var _))
        {
            GameManager.Instance.dialogueManager.PlayDialogById(dialogueId);
            Destroy(gameObject);
        }
    }
}
