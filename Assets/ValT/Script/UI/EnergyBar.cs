using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyBar : MonoBehaviour
{
    [Header("Bar Components")]
    [SerializeField] private Image fastLayer; // Layer qui descend rapidement
    [SerializeField] private Image slowLayer; // Layer qui descend lentement

    [Header("Holder Settings")]
    [SerializeField] private RectTransform holder; // Le conteneur qui contient les deux layers

    [Header("Floating Settings")]
    [SerializeField] private float floatingRange; // Amplitude des mouvements constants
    [SerializeField] private float floatingSpeed; // Vitesse des mouvements constants

    [Header("Shake Settings")]
    [SerializeField] private float shakeIntensity; // Intensité des oscillations pendant l'agitation
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float shakeDuration; // Durée des oscillations pendant l'agitation

    [Header("Animation Settings")]
    [SerializeField] private float enlargeAmount; // Facteur de grossissement
    [SerializeField] private Vector3 screenCenterOffset; // Décalage vers le centre de l'écran
    [SerializeField] private float firstLayerDelay;
    [SerializeField] private float growSpeed; // Vitesse pour revenir à la position/taille d'origine
    [SerializeField] private float returnSpeed; // Vitesse pour revenir à la position/taille d'origine

    [Header("Animator Value")]
    [SerializeField] private Animator animator;
    [SerializeField] private float idleLightningTime;
    [SerializeField] private float overchargeTime;

    private float overchargeTimer;


    [Header("Slow Layer Settings")]
    [SerializeField] private float fastLayerDuration; // Durée de l'animation du fastLayer
    [SerializeField] private float slowLayerDelay; // Délai avant que le 2e layer descende
    [SerializeField] private float slowLayerSpeed; // Vitesse du 2e layer

    [Header("Fast Layer Settings")]
    [SerializeField] private AnimationCurve fastLayerCurve; // Courbe d'interpolation pour le fastLayer

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
        StartCoroutine(IdleLightning());
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
        
        while (Vector3.Distance(holder.localScale, originalScale * enlargeAmount) > 0.01f ||
               Vector3.Distance(holder.position, enlargedPosition) > 0.01f)
        {
            holder.localScale = Vector3.Lerp(holder.localScale, originalScale * enlargeAmount, Time.deltaTime * growSpeed);
            holder.position = Vector3.Lerp(holder.position, enlargedPosition, Time.deltaTime * growSpeed);
            yield return null;
        }


        holder.localScale = originalScale * enlargeAmount;
        holder.position = enlargedPosition;

        // Étape 2 : Tremblement intense (agitation)
        StartCoroutine(ShakeEffect(enlargedPosition));
        isShaking = true;
        StartCoroutine(OverchargeBar());
        yield return new WaitForSeconds(firstLayerDelay);

        // Étape 3 : Animation du fastLayer (avec courbe d'interpolation)
        float initialFastValue = currentFastValue;
        float elapsedFastTime = 0f;

        while (elapsedFastTime < fastLayerDuration)
        {
            elapsedFastTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedFastTime / fastLayerDuration);
            currentFastValue = Mathf.Lerp(initialFastValue, targetValue, fastLayerCurve.Evaluate(t));
            fastLayer.fillAmount = currentFastValue;
            yield return null;
        }

        currentFastValue = targetValue;
        fastLayer.fillAmount = currentFastValue;

        // Étape 4 : Animation du slowLayer après un délai
        StartCoroutine(AnimateSlowLayer(targetValue));
        yield return new WaitForSeconds(slowLayerDelay);

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
        StartCoroutine(IdleLightning());
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

    private IEnumerator ShakeEffect(Vector3 initialPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration && isShaking)
        {
            elapsedTime += shakeSpeed;

            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);

            holder.position = initialPosition + new Vector3(offsetX, offsetY, 0);
            yield return new WaitForSeconds(shakeSpeed);
        }

        holder.position = initialPosition;
    }

    private IEnumerator OverchargeBar()
    {
        animator.SetBool("IsOvercharged", true);
        overchargeTimer = overchargeTime;
        while(overchargeTimer > 0)
        {
            overchargeTimer -= Time.deltaTime;
            yield return null;
        }
        animator.SetBool("IsOvercharged", false);
    }

    private IEnumerator IdleLightning()
    {
        while(true)
        {
            yield return new WaitForSeconds(idleLightningTime);
            animator.SetTrigger("PlayIdle");
        }
    }
}
