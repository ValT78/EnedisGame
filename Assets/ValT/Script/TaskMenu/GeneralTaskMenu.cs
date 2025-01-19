using UnityEngine;

public class GeneralTaskMenu : MonoBehaviour
{
    [SerializeField] private int taskPoints;
    [SerializeField] private string successDialogueId;

    public void CompleteTask()
    {
        print("Task completed!");
        GameManager.Instance.CompleteTask(taskPoints);
        if(successDialogueId != "") GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        Destroy(gameObject);
    }
}
