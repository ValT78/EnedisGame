using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Unique Objects")]
    public EnergyBar energyBar;
    public FirstPersonController playerController;
    public DialogueManager dialogueManager;
    public AudioManager audioManager;
    public SequenceManager sequenceManager;

    [Header("Game Parameters")]
    [SerializeField] private int maxScore;
    public int startChronoTime;

    [Header("Active Game Settings")]
    public static bool isPaused = false;
    public static bool isIntro = true;
    public static int lastInteractedObject = 0;
    public static bool inAnimation = false;

    private int score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = maxScore;
    }

    public void CompleteTask(int taskPoints)
    {
        score -= taskPoints;
        if (taskPoints > 0) energyBar.ChangeBarValue((float)score / maxScore);
        playerController.CompleteTask();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

}
