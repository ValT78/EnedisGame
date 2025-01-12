using UnityEngine;

public class GeneralTaskMenu : MonoBehaviour
{
    [SerializeField] private int taskPoints;
    public void CompleteTask()
    {
        GameManager.Instance.CompleteTask(taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById("intro_1");
        Destroy(gameObject);
    }
}
