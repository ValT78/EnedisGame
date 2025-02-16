using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string dialogId;
    public string speaker;
    public string text;
    public AudioClip audioClip;
    public Sprite characterSprite;
    public Vector3 characterPosition;
    public Vector3 characterRotation;
    public bool autoSkip;
}