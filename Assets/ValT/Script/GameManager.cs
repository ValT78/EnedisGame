using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Unique Objects")]
    public EnergyBar energyBar;
    public FirstPersonController playerController;
    public DialogueManager dialogueManager;
    public AudioManager audioManager;

    [Header("Game Parameters")]
    [SerializeField] private int maxScore;

    [Header("Intro Animation Settings")]
    [SerializeField] private Vector3 startRotation; // Rotation de départ (Euler)
    [SerializeField] private Vector3 endRotation;   // Rotation de fin (Euler)
    [SerializeField] private Image fadeImage;   // Image noire pour le fondu
    [SerializeField] private float introAnimationDuration;
    [SerializeField] private string introDialogueId;

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

    public void StartGame()
    {
        isIntro = false;

        energyBar.gameObject.SetActive(true);

        foreach (InteractableObject obj in FindObjectsByType<InteractableObject>(FindObjectsSortMode.None))
        {
            if(obj.GetResetAfterIntro())
            {
                obj.TurnIntoNotIntroObject();
            }
        }
    }

    public IEnumerator StartIntro()
    {
        isIntro = true;
        inAnimation = true;
        StartCoroutine(playerController.PlayIntroAnimation(introAnimationDuration, startRotation, endRotation, fadeImage));
        yield return new WaitForSeconds(introAnimationDuration);
        dialogueManager.PlayDialogById(introDialogueId);    
        inAnimation = false;

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
