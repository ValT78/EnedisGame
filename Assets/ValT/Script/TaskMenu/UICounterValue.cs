using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UICounterValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCounter;

    [Header("Getter")]
    [SerializeField] private RotatingDial valueGetter; // L'événement déclenché

    void Update()
    {
        if(valueGetter!=null) textCounter.text = valueGetter.GetValue().ToString() + "°";
    }
}
