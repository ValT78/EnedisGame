using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] private List<GameObject> currentCards;
    [SerializeField] private List<CardInfo> goodCardCombinaison;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardsY = 0;
    [SerializeField] private DialogueManagerQuartier dialogueManager;
    [SerializeField] private JaugeManager jaugeManager;
    [SerializeField] private CardInfo cardToAdd;
    private float canvaWidth;
    private float canvaHeight;
    private int nbrGoodAnswers = 5;
    private int currentNbrGoodAnswers = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvaWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvaHeight = canvas.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCards(List<CardInfo> cardsInfos, List<CardInfo> rightCombinaison)
    {
        currentCards.Clear();
        goodCardCombinaison.Clear();
        foreach (CardInfo cardInfo in cardsInfos)
        {
            GameObject card = Instantiate(cardPrefab, canvas.transform);
            card.GetComponent<Card>().SetCardInfo(cardInfo);
            currentCards.Add(card);
        }
        UpdatePositionsCardsOnScreen();

        foreach(CardInfo cardInfo in rightCombinaison)
        {
            goodCardCombinaison.Add(cardInfo);
        }
        nbrGoodAnswers = rightCombinaison.Count;
        currentNbrGoodAnswers = 0;
    }

    private void UpdatePositionsCardsOnScreen()
    {
        int n_cards = currentCards.Count;
        float range = (4f / 5f) * canvaWidth;
        float h = range/5f;
        Vector2 basePosition = new Vector2(-(h / 2f) * (n_cards-1), cardsY);
        for(int i=0; i<n_cards; i++)
        {
            GameObject card = currentCards[i];
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchoredPosition = basePosition + (new Vector2(h*i, 0));
        }
    }

    public void removeCard(GameObject card)
    {
        currentCards.Remove(card);
        foreach(CardInfo cardInfoGood in goodCardCombinaison)
        {
            CardInfo cardInfo = card.GetComponent<Card>().getCardInfo();
            if (cardInfo.name == cardInfoGood.name)
            {
                currentNbrGoodAnswers += 1;
            }
        }
        Destroy(card);
        UpdatePositionsCardsOnScreen();
    }

    public void setCardsActive(bool active)
    {
        foreach(GameObject card in currentCards)
        {
            card.SetActive(active);
        }
    }

    public bool goodCombinationFound()
    {
        return (currentNbrGoodAnswers == nbrGoodAnswers);
    }

    public void cardClicked(GameObject card, string[] repliques, int jaugeAmount)
    {     
        jaugeManager.changeJauge(jaugeAmount);
        StartCoroutine(waitWhileDialogueCoroutine(repliques, card));
    }

    private IEnumerator waitWhileDialogueCoroutine(string[] repliques, GameObject card)
    {
        setCardsActive(false);
        yield return new WaitUntil(() => !jaugeManager.getIsChanging());
        dialogueManager.startDialogue(repliques);
        yield return new WaitUntil(() => !dialogueManager.isTalking());
        if (card.GetComponent<Card>().getCardInfo().name == "Subventionner les panneaux solaires") 
        {
            GameObject newCard = Instantiate(cardPrefab, canvas.transform);
            newCard.GetComponent<Card>().SetCardInfo(cardToAdd);
            currentCards.Add(newCard);
        }
        removeCard(card);
        if (goodCombinationFound() && (currentCards.Count != 0))
        {
            foreach (GameObject cardToRemove in currentCards)
            {
                Destroy(cardToRemove);
            }
            currentCards.Clear();
            UpdatePositionsCardsOnScreen();
        }
        setCardsActive(true);
    } 
}
