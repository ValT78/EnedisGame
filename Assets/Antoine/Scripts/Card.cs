using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardInfo cardInfo;
    private Image cardBorder;
    [SerializeField] private Image cardInterior;
    [SerializeField] private Image imageBorder;
    [SerializeField] private Image textBorder;
    [SerializeField] private Image textContenant;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI titre;
    [SerializeField] private float baseSize = 1f;
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] private float sizeChangeDuration = 0.3f;
    private CardManager cardManager;
    private bool sizeIsBig = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardManager = Object.FindFirstObjectByType(typeof(CardManager)).GetComponent<CardManager>();
        cardBorder = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardInfo(CardInfo cardInfo)
    {
        this.cardInfo = cardInfo;
        if (cardBorder == null)
        {
            cardBorder = GetComponent<Image>();
        }
        cardBorder.color = cardInfo.borderColor;
        cardInterior.color = cardInfo.color;
        imageBorder.color = cardInfo.borderColor;
        textBorder.color = cardInfo.borderColor;
        textContenant.color = cardInfo.textContenantColor;
        titre.text = cardInfo.name;
        image.sprite = cardInfo.image;
    }

    public CardInfo getCardInfo()
    {
        return this.cardInfo;
    }

    private IEnumerator sizeUpCoroutine()
    {
        sizeIsBig = true;
        float t = 0;
        RectTransform rectTransform = GetComponent<RectTransform>();
        while (t<1)
        {
            t += Time.deltaTime / sizeChangeDuration;
            rectTransform.localScale = new Vector3(Mathf.Lerp(baseSize, maxSize, t), Mathf.Lerp(baseSize, maxSize, t), rectTransform.localScale.z);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator sizeDownCoroutine()
    {
        sizeIsBig = false;
        float t = 0;
        RectTransform rectTransform = GetComponent<RectTransform>();
        while (t < 1)
        {
            t += Time.deltaTime / sizeChangeDuration;
            rectTransform.localScale = new Vector3(Mathf.Lerp(maxSize, baseSize, t), Mathf.Lerp(maxSize, baseSize, t), rectTransform.localScale.z);
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Object dialogueManager = Object.FindFirstObjectByType(typeof(DialogueManagerQuartier));
        if (!sizeIsBig)
        {
            StartCoroutine(sizeUpCoroutine());
            dialogueManager.GetComponent<DialogueManagerQuartier>().showDescription(cardInfo.description, cardInfo.textContenantColor, cardInfo.borderColor);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Object dialogueManager = Object.FindFirstObjectByType(typeof(DialogueManagerQuartier));
        if (sizeIsBig)
        {
            StartCoroutine(sizeDownCoroutine());
            dialogueManager.GetComponent<DialogueManagerQuartier>().hideDescription();
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Object dialogueManager = Object.FindFirstObjectByType(typeof(DialogueManagerQuartier));
        dialogueManager.GetComponent<DialogueManagerQuartier>().hideDescription();
        cardManager.cardClicked(this.gameObject, cardInfo.repliquesSiChoisi, cardInfo.jaugeAmount);
    }
}
