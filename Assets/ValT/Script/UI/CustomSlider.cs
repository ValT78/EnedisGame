using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class CustomSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Slider Elements")]
    public Image sliderBackground; // Image de fond du slider
    public Image fillImage;        // Image qui se remplit

    [Header("Settings")]
    [Range(0, 1)] public float value = 0f; // Valeur actuelle du slider (entre 0 et 1)
    public float fillSpeed = 10f;          // Vitesse de remplissage
    public float glowFadeSpeed = 5f;       // Vitesse d'apparition/disparition du glow

    [Header("Callbacks")]
    public UnityEvent<float> onValueChanged; // Appelle cette fonction quand la valeur change

    [Header("Visual Effects")]
    public Gradient fillColorGradient;       // Gradient pour la couleur de la barre
    public Outline glowEffect;              // Effet de glow autour de la barre
    public TextMeshProUGUI valueText;                  // Texte dynamique affichant la valeur

    private RectTransform sliderRect;
    private float targetValue = 0f;         // Valeur cible pour l'animation
    private Coroutine glowCoroutine;        // Référence pour contrôler la coroutine du glow

    private void Start()
    {
        if (sliderBackground != null)
        {
            sliderRect = sliderBackground.GetComponent<RectTransform>();
        }
        UpdateFill();
    }

    private void Update()
    {
        // Animation de remplissage fluide
        if (Mathf.Abs(value - targetValue) > 0.001f)
        {
            value += (targetValue - value) / fillSpeed; // Animation de lissage
            UpdateFill();
        }
    }

    // Met à jour la barre de remplissage
    private void UpdateFill()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = value; // Met à jour le remplissage

            // Met à jour la couleur en fonction du gradient
            if (fillColorGradient != null)
            {
                fillImage.color = fillColorGradient.Evaluate(value);
            }
        }

        // Met à jour le texte (si présent)
        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    // Appelé lorsque la souris clique sur le slider
    public void OnPointerDown(PointerEventData eventData)
    {

        UpdateSliderValue(eventData);

        // Activer le glow progressif
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        glowCoroutine = StartCoroutine(FadeGlow(1f)); // Apparition du glow
    }

    // Appelé lorsque la souris glisse sur le slider
    public void OnDrag(PointerEventData eventData)
    {
        UpdateSliderValue(eventData);
    }

    // Appelé lorsque la souris relâche le slider
    public void OnPointerUp(PointerEventData eventData)
    {
        // Désactiver le glow progressif
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        glowCoroutine = StartCoroutine(FadeGlow(0f)); // Disparition du glow
    }

    // Calcule et met à jour la valeur cible du slider en fonction de la position de la souris
    private void UpdateSliderValue(PointerEventData eventData)
    {
        if (sliderRect == null)
            return;

        // Obtenir la position de la souris par rapport au slider
        Vector2 localMousePosition = sliderRect.InverseTransformPoint(eventData.position);

        // Calculer la valeur entre 0 et 1
        float sliderWidth = sliderRect.rect.width;
        targetValue = Mathf.Clamp01((localMousePosition.x / sliderWidth) + 0.5f);

        // Appeler le callback (si défini dans l'inspecteur)
        onValueChanged?.Invoke(targetValue);
    }

    // Coroutine pour gérer l'apparition/disparition progressive du glow
    private IEnumerator FadeGlow(float targetAlpha)
    {
        if (glowEffect == null)
            yield break;

        float currentAlpha = glowEffect.effectColor.a;

        
        while (Mathf.Abs(targetAlpha - currentAlpha) > 0.001f)
        {

            currentAlpha += (targetAlpha - currentAlpha) / glowFadeSpeed;
            Color newColor = glowEffect.effectColor;
            newColor.a = currentAlpha;
            glowEffect.effectColor = newColor;
            yield return null;
        }
    }
}
