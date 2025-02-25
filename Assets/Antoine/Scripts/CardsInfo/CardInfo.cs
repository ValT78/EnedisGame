using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardInfo", order = 1)]
public class CardInfo : ScriptableObject
{
    public string name;
    public string description;
    public int jaugeAmount;
    public Sprite image;
    public Color color;
    public Color borderColor;
    public Color textContenantColor;
    public string[] repliquesSiChoisi;
}
