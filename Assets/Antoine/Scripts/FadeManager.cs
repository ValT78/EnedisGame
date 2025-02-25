using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeScreenImage;
    [SerializeField] private float fadeDuration = 1.0f;
    private bool screenBlack = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fade(bool fadeIn)
    {
        StartCoroutine(fadeCoroutine(fadeIn));
    }

    IEnumerator fadeCoroutine(bool fadeIn)
    {
        float t = 0;
        Color newColor = fadeScreenImage.color;
        while (t<1)
        {
            t += Time.deltaTime/fadeDuration;
            if (fadeIn)
            {
                newColor.a = Mathf.Lerp(1f, 0, t);
            } else
            {
                newColor.a = Mathf.Lerp(0, 1f, t);
            }
            fadeScreenImage.color = newColor;
            yield return new WaitForEndOfFrame();
        }
        if (fadeIn)
        {
            screenBlack = false;
        } else
        {
            screenBlack = true;
        }
    } 

    public bool isScreenBlack()
    {
        return screenBlack;
    }
}
