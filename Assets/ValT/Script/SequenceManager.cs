using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour
{
    [Header("Intro Animation Settings")]
    [SerializeField] private Vector3 startRotation; // Rotation de départ (Euler)
    [SerializeField] private Vector3 endRotation;   // Rotation de fin (Euler)
    [SerializeField] private Image fadeImage;   // Image noire pour le fondu
    [SerializeField] private float introAnimationDuration;
    [SerializeField] private string introDialogueId;


    public static SequenceManager Instance;

    private void Start()
    {
        StartCoroutine(StartIntro());
    }

    public IEnumerator StartGame()
    {
        GameManager.isIntro = false;
        GameManager.inAnimation = true;
        yield return new WaitForSeconds(1f);

        GameManager.Instance.energyBar.gameObject.SetActive(true);

        foreach (InteractableObject obj in FindObjectsByType<InteractableObject>(FindObjectsSortMode.None))
        {
            if (obj.GetResetAfterIntro())
            {
                obj.TurnIntoNotIntroObject();
            }
        }


        LightSwitchInteractible.ResetAllLights(true);
        GameManager.inAnimation = false;
    }

    public IEnumerator ShutLight()
    {
        GameManager.inAnimation = true;
        LightSwitchInteractible.ResetAllLights(false);
        yield return new WaitForSeconds(1f);
        GameManager.inAnimation = false;
    }

    public IEnumerator StartIntro()
    {
        GameManager.isIntro = false;
        GameManager.inAnimation = true;
        StartCoroutine(GameManager.Instance.playerController.PlayIntroAnimation(introAnimationDuration, startRotation, endRotation, fadeImage));
        yield return new WaitForSeconds(introAnimationDuration);
        GameManager.Instance.dialogueManager.PlayDialogById(introDialogueId);
        GameManager.inAnimation = false;

    }
}
