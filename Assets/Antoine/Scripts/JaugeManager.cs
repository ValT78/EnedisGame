using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JaugeManager : MonoBehaviour
{
    private float taux = 0;
    [SerializeField] private float tauxAiguille = 0;
    [SerializeField] private float maxTaux = 100;
    private bool isChanging = false;
    private enum ChangeIntensity {faible, moyenne, forte};
    [SerializeField] private float changementMoyen = 30;
    private float var = 0;
    [SerializeField] private float maxVar = 10;
    [SerializeField] private float dureeTransition = 1;
    [SerializeField] private float dureeTransitionQuick = 0.5f;
    [SerializeField] private float dureeTransitionVar = 0.5f;
    [SerializeField] private Image imageAiguilleTaux;
    [SerializeField] private Image imageCadranJauge;
    [SerializeField] private float endChangeWaitDuration = 0.5f;
    [SerializeField] private float showJaugeClignotementDuration = 0.1f;
    [SerializeField] private float showJaugeDuration = 1f;
    private bool jaugeVisible = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ChangeVarCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTaux();
        DisplayJauge();
    }

    private void DisplayJauge()
    {
        float maxAngle = 90;
        float angle = -(tauxAiguille / maxTaux) * maxAngle;
        imageAiguilleTaux.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator ChangeTauxCoroutine(int jaugeAmount)
    {
        isChanging = true;
        float signe;
        float t = 0;
        float start = taux;
        float end = start + jaugeAmount;
        while (taux!=end)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime / dureeTransition;
            taux = Mathf.Lerp(start, end, t);
        }
        yield return new WaitForSeconds(endChangeWaitDuration);
        isChanging = false;
    }

    public void changeJauge(int jaugeAmount)
    {
        StartCoroutine(ChangeTauxCoroutine(jaugeAmount));
    }

    private IEnumerator ChangeTauxCoroutineInstant(int startJauge, float duration)
    {
        isChanging = true;
        float t = 0;
        float start = taux;
        float end = startJauge;
        while (t<1)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime / duration;
            taux = Mathf.Lerp(start, end, t);
        }
        yield return new WaitForSeconds(endChangeWaitDuration);
        isChanging = false;
    }

    public void changeJaugeInstant(int startJauge, float duration)
    {
        StartCoroutine(ChangeTauxCoroutineInstant(startJauge, duration));
    }

    private IEnumerator ChangeVarCoroutine()
    {
        float sens = -1;
        while (true)
        {
            sens = -sens;
            float start = var;
            float end = Random.Range(Mathf.Min(start, sens*maxVar), Mathf.Max(start, sens*maxVar));
            if (isChanging)
            {
                end = 0;
            }
            float t = 0;
            while (t<1)
            {
                t += Time.deltaTime / dureeTransitionVar;
                var = Mathf.Lerp(start, end, t);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void UpdateTaux()
    {
        tauxAiguille = taux + var;
    }

    public bool getIsChanging()
    {
        return isChanging;
    }

    public void showJauge()
    {
        StartCoroutine(showJaugeCoroutine());
    }

    public bool isJaugeVisible()
    {
        return jaugeVisible;
    }

    IEnumerator showJaugeCoroutine()
    {
        Color transparent = Color.white;
        transparent.a = 0;

        imageAiguilleTaux.color = Color.white;
        imageCadranJauge.color = Color.white;
        yield return new WaitForSeconds(showJaugeClignotementDuration);
        imageAiguilleTaux.color = transparent;
        imageCadranJauge.color = transparent;
        yield return new WaitForSeconds(showJaugeClignotementDuration);
        imageAiguilleTaux.color = Color.white;
        imageCadranJauge.color = Color.white;
        yield return new WaitForSeconds(showJaugeDuration);
        jaugeVisible = true;
    }
}
