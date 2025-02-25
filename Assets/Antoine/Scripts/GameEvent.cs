using System;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using System.Collections.Generic;

public enum TypeEvent { TimeChange, Dialogue, Choice, Wait, ShowJauge, Fade }

[Serializable]
public class GameEvent
{
    public string name;

    public TypeEvent type;

    public int newHour;
    public int newMinute;
    public float changeTimeDuration;
    public int startJauge;

    public string[] lstRepliques;

    public List<CardInfo> cardsInfo;
    public List<CardInfo> goodCombinaison;

    public float waitDuration;

    public bool fadeIn;
}
