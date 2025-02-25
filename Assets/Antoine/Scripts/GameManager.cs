using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManagerQuartier : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    [SerializeField] private DialogueManagerQuartier dialogueManager;
    [SerializeField] private ClockManager clockManager;
    [SerializeField] private JaugeManager jaugeManager;
    [SerializeField] private FadeManager fadeManager;
    [SerializeField] private List<GameEvent> gameSequence;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(playGameSequenceCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator playGameSequenceCoroutine()
    {
        foreach (GameEvent gameEvent in gameSequence)
        {
            if (gameEvent.type==TypeEvent.Dialogue)
            {
                dialogueManager.startDialogue(gameEvent.lstRepliques);
                yield return new WaitUntil(() => !dialogueManager.isTalking());
            } else if (gameEvent.type==TypeEvent.TimeChange)
            {
                clockManager.changeHour(gameEvent.newHour, gameEvent.newMinute, gameEvent.changeTimeDuration);
                jaugeManager.changeJaugeInstant(gameEvent.startJauge, gameEvent.changeTimeDuration);
                yield return new WaitForSeconds(gameEvent.changeTimeDuration);
            } else if (gameEvent.type == TypeEvent.Choice)
            {
                cardManager.InitializeCards(gameEvent.cardsInfo, gameEvent.goodCombinaison);
                yield return new WaitUntil(() => cardManager.goodCombinationFound());
            } else if (gameEvent.type==TypeEvent.Wait)
            {
                yield return new WaitForSeconds(gameEvent.waitDuration);
            } else if (gameEvent.type == TypeEvent.ShowJauge)
            {
                jaugeManager.showJauge();
                yield return new WaitUntil(() => jaugeManager.isJaugeVisible());
            } else if (gameEvent.type == TypeEvent.Fade)
            {
                fadeManager.fade(gameEvent.fadeIn);
                yield return new WaitUntil(() => fadeManager.isScreenBlack()!=gameEvent.fadeIn);
            }
        }
    }
}
