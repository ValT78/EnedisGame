using UnityEngine;

public class GeneralTaskMenu : MonoBehaviour
{
    private int taskPoints;
    private string successDialogueId;
    private int interactSequenceOrder;

    private GameObject UIHolder;

    private void Start()
    {
        // Trouver le holder de l'UI avec le tag UIHolder
        UIHolder = GameObject.FindGameObjectWithTag("UIHolder");
        if(UIHolder != null) UIHolder.SetActive(false);
    }

    public void ConfigureMenu(int taskPoints, string successDialogueId, int interactSequenceOrder)
    {
        this.taskPoints = taskPoints;
        this.successDialogueId = successDialogueId;
        this.interactSequenceOrder = interactSequenceOrder;
    }

    public void CompleteTask()
    {
        UIHolder.SetActive(true);
        GameManager.Instance.CompleteTask(taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        GameManager.lastInteractedObject = interactSequenceOrder;
        Destroy(gameObject);
    }

    public void CancelTask()
    {
        UIHolder.SetActive(true);
        Destroy(gameObject);
    }
}
