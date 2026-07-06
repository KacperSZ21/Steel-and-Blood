using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WhiteScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        var c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    public IEnumerator FadeWhite()
    {
        // Fade IN 
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = t / fadeDuration;
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(1f);

        // a short break to give the camera time to switch over
        yield return new WaitForSeconds(0.1f);

        // Fade OUT
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = 1f - (t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(0f);
    }

    void SetAlpha(float value)
    {
        var c = fadeImage.color;
        c.a = value;
        fadeImage.color = c;
    }
}