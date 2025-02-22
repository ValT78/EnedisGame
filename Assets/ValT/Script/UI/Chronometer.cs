using UnityEngine;
using TMPro;
using System.Collections;

public class Chronometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // Texte du chrono
    [SerializeField] private RectTransform rectTransform; // Image du chrono

    [SerializeField] private RectTransform startPosition;
    [SerializeField] private RectTransform centerPosition;
    [SerializeField] private RectTransform finalPosition;

    private float timeRemaining;
    private bool isRunning = false;
    private Vector3 originalPosition;

    [SerializeField] private float moveDuration; // Durée du déplacement
    [SerializeField] private float blinkDuration; // Durée du clignotement
    [SerializeField] private int blinkCount; // Nombre de clignotements
    [SerializeField] private float floatAmplitude; // Intensité du flottement
    [SerializeField] private float floatSpeed; // Vitesse du flottement
    [SerializeField] private float floatRate; // Vitesse du flottement
    [SerializeField] private float shrinkDuration; // Durée du redimensionnement

    void Start()
    {
        originalPosition = centerPosition.position;
        timeRemaining = GameManager.Instance.startChronoTime;
        UpdateTimerText();

        // Position initiale hors écran
        transform.position = startPosition.position;
        StartCoroutine(ChronometerSequence());
    }

    private IEnumerator ChronometerSequence()
    {
        yield return StartCoroutine(MoveToPosition(centerPosition.position, moveDuration));
        StartCoroutine(FloatEffect());
        yield return StartCoroutine(BlinkText());
        yield return StartCoroutine(ShrinkToCorner());
        yield return StartCoroutine(ChronoRoutine());

    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        float elapsedTime = 0;
        Vector3 start = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator BlinkText()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            timerText.alpha = (timerText.alpha == 1) ? 0 : 1;
            yield return new WaitForSeconds(blinkDuration / blinkCount);
        }
        timerText.alpha = 1;
    }

    private IEnumerator FloatEffect()
    {
        while (true)
        {
            float offsetX = Mathf.PerlinNoise(Time.time * floatSpeed, 0f) * 2 - 1; // Bruit de Perlin pour un mouvement fluide
            float offsetY = Mathf.PerlinNoise(0f, Time.time * floatSpeed) * 2 - 1;

            Vector3 floatingPosition = originalPosition + new Vector3(offsetX, offsetY, 0) * floatAmplitude;

            float t = floatRate;
            while (t > 0)
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position, floatingPosition, t/floatRate);
                t -= Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator ChronoRoutine()
    {
        isRunning = true;
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1;
            UpdateTimerText();

            if (timeRemaining % 5 == 0)
                StartCoroutine(PulseEffect());
        }
        isRunning = false;
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private IEnumerator PulseEffect()
    {
        float duration = 0.3f;
        float scaleFactor = 1.2f;
        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            yield return null;
        }
    }

    private IEnumerator ShrinkToCorner()
    {
        StopCoroutine(FloatEffect()); // Arrête l’effet de flottement

        float elapsedTime = 0;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * 0.4f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = finalPosition.position;

        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / shrinkDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPos;

        // Modifier le centre du flottement
        originalPosition = targetPos;
        StartCoroutine(FloatEffect());
    }
}
