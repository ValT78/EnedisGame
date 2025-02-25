using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerQuartier : MonoBehaviour
{
    [SerializeField] private Image dialogueBox;
    [SerializeField] private Image dialogueBoxBorder;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float tShort;
    [SerializeField] private float tMedium;
    [SerializeField] private float tLong;
    [SerializeField] private char[] lstCharacterLong;
    [SerializeField] private char[] lstCharacterMedium;
    [SerializeField] private char[] lstCharacterInstant;
    private bool action;
    private bool talking = false;
    private Color baseColorDialoguebox;
    private Color baseColorDialogueboxBorder;

    [SerializeField] private string[] RepliquesTest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseColorDialoguebox = dialogueBox.color;
        baseColorDialogueboxBorder = dialogueBoxBorder.color;
        HideDialogueBox();
        action = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( (Input.GetKeyDown(KeyCode.Space))||(Input.GetKeyDown(KeyCode.Mouse0)) )
        {
            action = true;
        }
    }

    private void HideDialogueBox()
    {
        Color trensparent = Color.white;
        trensparent.a = 0;
        dialogueBox.color = trensparent;
        dialogueBoxBorder.color = trensparent;

        dialogueText.text = "";

        talking = false;
    }

    private void ShowDialogueBox(Color color, Color colorBorder)
    {
        dialogueBox.color = color;
        dialogueBoxBorder.color = colorBorder;
        talking = true;
    }

    IEnumerator DisplayDialoguesCoroutine(string[] repliques)
    {
        int length;
        int maxLength;
        int nbrRepliques = repliques.Length;
        int indexReplique = 0;

        ShowDialogueBox(baseColorDialoguebox, baseColorDialogueboxBorder);
        while (indexReplique<nbrRepliques) {
            string replique = repliques[indexReplique];
            maxLength = replique.Length;
            length = 0;
            action = false;
            while (length<maxLength)
            {
                length += 1;
                if (action)
                {
                    action = false;
                    length = maxLength;
                }
                dialogueText.text = replique.Substring(0, length);
                char character = replique[length-1];
                yield return new WaitForSeconds(characterToTime(character));
            }
            yield return new WaitUntil(() => action);
            indexReplique += 1;
        }
        HideDialogueBox();
    }

    private float characterToTime(char character)
    {
        if (lstCharacterLong.Contains(character))
        {
            return tLong;
        } else if (lstCharacterMedium.Contains(character))
        {
            return tMedium;
        } else if (lstCharacterInstant.Contains(character))
        {
            return 0;
        } else
        {
            return tShort;
        }
    }

    public void showDescription(string description, Color color, Color colorBorder)
    {
        Color newColor = color;
        newColor.a = baseColorDialoguebox.a;
        ShowDialogueBox(newColor, colorBorder);
        dialogueText.text = description;
    }

    public void hideDescription()
    {
        dialogueBox.color = baseColorDialoguebox;
        HideDialogueBox();
    }

    public bool isTalking()
    {
        return talking;
    }

    public void startDialogue(string[] repliques)
    {
        StartCoroutine(DisplayDialoguesCoroutine(repliques));
    }
}
