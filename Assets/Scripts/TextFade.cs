using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float delayBeforeFade = 1f;       // Thoi gian delay truoc khi bat dau fade
    public float fadeDuration = 1f;          // Thoi gian fade tu 0 -> 1

    private Text uiText;

    void Awake()
    {
        uiText = GetComponent<Text>();
        if (uiText != null)
        {
            Color c = uiText.color;
            c.a = 0f;
            uiText.color = c;
        }
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);

            if (uiText != null)
            {
                Color c = uiText.color;
                c.a = alpha;
                uiText.color = c;
            }

            yield return null;
        }
    }
}
