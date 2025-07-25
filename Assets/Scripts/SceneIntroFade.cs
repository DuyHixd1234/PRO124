using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneIntroFade : MonoBehaviour
{
    public Image blackPanel;
    public float fadeDuration = 2f;
    private bool hasFaded = false;

    public delegate void FadeComplete();
    public static event FadeComplete OnFadeDone;

    void OnEnable()
    {
        hasFaded = false;

        if (blackPanel != null)
        {
            // Reset màu ?en
            Color c = blackPanel.color;
            blackPanel.color = new Color(c.r, c.g, c.b, 1f);
            blackPanel.gameObject.SetActive(true);
        }

        StartCoroutine(FadeOutBlackPanel());
    }

    IEnumerator FadeOutBlackPanel()
    {
        float t = 0f;
        Color c = blackPanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            blackPanel.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        blackPanel.gameObject.SetActive(false);

        if (!hasFaded)
        {
            hasFaded = true;
            OnFadeDone?.Invoke();
        }
    }
}
