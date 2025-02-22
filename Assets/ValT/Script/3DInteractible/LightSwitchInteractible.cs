using NUnit.Framework;
using UnityEngine;

public class LightSwitchInteractible : InteractableObject
{
    private static System.Collections.Generic.List<LightSwitchInteractible> allLightSwitches = new();

    [Header("Light Settings")]
    [SerializeField] private Light[] targetLights; // Lumière à activer/désactiver

    private void Awake()
    {
        allLightSwitches.Add(this);
    }

    public override bool Interact()
    {
        //On active/désactive les lumières
        foreach (Light light in targetLights)
        {
            light.enabled = !light.enabled;
        }

        //Gérer la partie task
        GameManager.Instance.CompleteTask(isIntro ? 0 : taskPoints);
        GameManager.Instance.dialogueManager.PlayDialogById(successDialogueId);
        GameManager.lastInteractedObject = interactSequenceOrder;

        //On active la base
        base.Interact();

        //On peut toujours bouger après avoir interagi avec l'objet²
        return false;
    }

    public static void ResetAllLights(bool state)
    {
        foreach (LightSwitchInteractible lightSwitch in allLightSwitches)
        {
            foreach (Light light in lightSwitch.targetLights)
            {
                light.enabled = state;
            }
        }
    }

}
