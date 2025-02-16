using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;


public class DialogueManager : MonoBehaviour
{
    public Dictionary<string, Dialogue> Dialogues = new();

    public TextMeshProUGUI dialogueText;
    public RectTransform characterTransform;
    public Image characterImage;
    public AudioSource audioSource;
    public GameObject dialoguePanel;

    private Queue<Dialogue> dialoguesQueue;
    private bool isDialogueActive = false;
    private Queue<string> currentDialogueSegments;

    public List<AudioClip> audioDialoguesList;
    public List<Sprite> spritesList;


    void Awake()
    {
        dialoguesQueue = new Queue<Dialogue>();
        currentDialogueSegments = new Queue<string>();
    }

    private void Start()
    {
        LoadDialoguesFromCSV("dialogues");
    }

    public bool GetIsDialogueActive()
    {
        return isDialogueActive;
    }

    void LoadDialoguesFromCSV(string fileName)
    {
        // Charge le fichier depuis Resources
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"Fichier CSV {fileName} non trouvé dans Resources.");
            return;
        }

        // Lit chaque ligne du fichier
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Ignore la première ligne (en-têtes)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(';');

            // Création d'un Dialogue à partir des champs CSV
            Dialogue dialogue = new Dialogue
            {
                speaker = fields[1],
                text = fields[2],
                audioClip = audioDialoguesList[i-1],
                characterSprite = spritesList[0],
                characterPosition = new Vector3(0,0,0),
                characterRotation = new Vector3(0,0,0),
                autoSkip = false
            };

            Dialogues.Add(fields[0], dialogue);
        }

        Debug.Log($"Chargé {Dialogues.Count} dialogues depuis le fichier CSV.");
    }

    public void PlayDialogById(string dialogId)
    {
        if (dialogId == "")
        {
            Debug.LogError("Aucun dialogue défini.");
            return;
        }
        Dialogue dialogue = Dialogues.GetValueOrDefault(dialogId);
        if (dialogue != null)
        {
            StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError($"Dialogue avec l'ID {dialogId} non trouvé.");
        }
    }

    public void SkipDialog()
    {
        if (currentDialogueSegments.Count > 0)
        {
            dialogueText.text = currentDialogueSegments.Dequeue();
        }

        else if (isDialogueActive)
        {
           /* redDialogueIndex++;
            print("Dialogue index: " + redDialogueIndex + " " + currentDialogueIndex);*/
            DisplayNextSentence();
        }
        

    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue.autoSkip)
        {
/*            redDialogueIndex += dialoguesQueue.Count;
*/            EndDialogue();
        }
        dialoguesQueue.Enqueue(dialogue);

        if (!isDialogueActive)
        {
            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            DisplayNextSentence();
        }
    }

    void DisplayNextSentence()
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        if (currentDialogueSegments.Count == 0)
        {
            if (dialoguesQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            Dialogue dialogue = dialoguesQueue.Dequeue();
            SplitDialogueText(dialogue.text);
            characterImage.sprite = dialogue.characterSprite;
           /* characterTransform.localPosition = dialogue.characterPosition;
            characterTransform.rotation = Quaternion.Euler(dialogue.characterRotation);*/

            audioSource.clip = dialogue.audioClip;
            audioSource.Play();
            print("Playing audio" + currentDialogueSegments.Count);
            dialogueText.text = currentDialogueSegments.Dequeue();


        }


    }

    void SplitDialogueText(string text)
    {
        currentDialogueSegments.Clear();
        int maxLength = 60; // Maximum length of each segment
        int start = 0; // Position de départ pour chaque segment

        while (start < text.Length)
        {
            int length = Mathf.Min(maxLength, text.Length - start); // Prend en compte la fin du texte
            int end = start + length;

            // Si on n'est pas à la fin, recherche le dernier espace avant `end`
            if (end < text.Length && text[end] != ' ')
            {
                int lastSpace = text.LastIndexOf(' ', end, length);
                if (lastSpace > start)
                {
                    end = lastSpace;
                }
            }

            // Ajoute le segment au dialogue
            currentDialogueSegments.Enqueue(text[start..end]);
            start = end + 1; // Repart après l'espace
        }
    }


    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentDialogueSegments.Clear(); // Clear the segments queue after ending the dialogue
        dialoguesQueue.Clear();


    }
}