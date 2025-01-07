using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyBar : MonoBehaviour
{
    [Header("Bar Components")]
    public Image fastLayer; // Layer qui descend rapidement
    public Image slowLayer; // Layer qui descend lentement

    [Header("Holder Settings")]
    public RectTransform holder; // Le conteneur qui contient les deux layers

    [Header("Floating Settings")]
    public float floatingRange; // Amplitude des mouvements constants
    public float floatingSpeed; // Vitesse des mouvements constants

    [Header("Shake Settings")]
    public float shakeIntensity; // Intensité des oscillations pendant l'agitation
    public float shakeDuration; // Durée des oscillations pendant l'agitation

    [Header("Animation Settings")]
    public float enlargeAmount; // Facteur de grossissement
    public Vector3 screenCenterOffset; // Décalage vers le centre de l'écran
    public float firstLayerDelay;
    public float returnSpeed; // Vitesse pour revenir à la position/taille d'origine

    [Header("Slow Layer Settings")]
    public float slowLayerDelay; // Délai avant que le 2e layer descende
    public float slowLayerSpeed; // Vitesse du 2e layer

    [Header("Fast Layer Settings")]
    public AnimationCurve fastLayerCurve; // Courbe d'interpolation pour le fastLayer

    private RectTransform rectTransform;
    private Vector3 originalHolderPosition;
    private Vector3 originalScale;
    private bool isShaking = false;

    private float currentFastValue = 1f;
    private float currentSlowValue = 1f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalHolderPosition = holder.position; // Position d'origine du conteneur
        originalScale = rectTransform.localScale;

        // Démarrer le mouvement constant du holder
        StartCoroutine(FloatingEffect());
    }

    /// <summary>
    /// Change la valeur de la barre avec animations.
    /// </summary>
    /// <param name="targetValue">Nouvelle valeur cible (entre 0 et 1).</param>
    public void ChangeBarValue(float targetValue)
    {
        StopAllCoroutines(); // Arrêter les animations en cours
        StartCoroutine(AnimateBar(targetValue));
    }

    private IEnumerator AnimateBar(float targetValue)
    {
        // Étape 1 : Stopper le flottement constant et grossir la barre
        StopCoroutine(FloatingEffect());
        Vector3 enlargedPosition = originalHolderPosition + screenCenterOffset;

        holder.localScale = originalScale * enlargeAmount;
        holder.position = enlargedPosition;

        // Étape 2 : Tremblement intense (agitation)
        isShaking = true;
        StartCoroutine(ShakeEffect());
        yield return new WaitForSeconds(firstLayerDelay);

        // Étape 3 : Animation du fastLayer (avec courbe d'interpolation)
        float initialFastValue = currentFastValue;
        float elapsedFastTime = 0f;
        float fastDuration = 0.5f; // Durée de l'animation rapide

        while (elapsedFastTime < fastDuration)
        {
            elapsedFastTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedFastTime / fastDuration);
            currentFastValue = Mathf.Lerp(initialFastValue, targetValue, fastLayerCurve.Evaluate(t));
            fastLayer.fillAmount = currentFastValue;
            yield return null;
        }

        currentFastValue = targetValue;
        fastLayer.fillAmount = currentFastValue;

        // Étape 4 : Animation du slowLayer après un délai
        yield return new WaitForSeconds(slowLayerDelay);
        StartCoroutine(AnimateSlowLayer(targetValue));

        // Étape 5 : Retour à la taille et position d'origine
        while (Vector3.Distance(holder.localScale, originalScale) > 0.01f ||
               Vector3.Distance(holder.position, originalHolderPosition) > 0.01f)
        {
            holder.localScale = Vector3.Lerp(holder.localScale, originalScale, Time.deltaTime * returnSpeed);
            holder.position = Vector3.Lerp(holder.position, originalHolderPosition, Time.deltaTime * returnSpeed);
            yield return null;
        }

        holder.localScale = originalScale;
        holder.position = originalHolderPosition;

        // Reprendre le flottement constant
        isShaking = false;
        StartCoroutine(FloatingEffect());
    }

    private IEnumerator AnimateSlowLayer(float targetValue)
    {
        float elapsedTime = 0f;

        while (Mathf.Abs(currentSlowValue - targetValue) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            currentSlowValue = Mathf.Lerp(currentSlowValue, targetValue, slowLayerSpeed * elapsedTime);
            slowLayer.fillAmount = currentSlowValue;
            yield return null;
        }

        currentSlowValue = targetValue;
        slowLayer.fillAmount = currentSlowValue;
    }

    private IEnumerator FloatingEffect()
    {
        while (true)
        {
            float offsetX = Mathf.PerlinNoise(Time.time * floatingSpeed, 0f) * 2 - 1; // Bruit de Perlin pour un mouvement fluide
            float offsetY = Mathf.PerlinNoise(0f, Time.time * floatingSpeed) * 2 - 1;

            Vector3 floatingPosition = originalHolderPosition + new Vector3(offsetX, offsetY, 0) * floatingRange;
            holder.position = Vector3.Lerp(holder.position, floatingPosition, Time.deltaTime * floatingSpeed);

            yield return null;
        }
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 initialPosition = holder.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration && isShaking)
        {
            elapsedTime += Time.deltaTime;

            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);

            holder.position = initialPosition + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        holder.position = initialPosition;
    }
}
