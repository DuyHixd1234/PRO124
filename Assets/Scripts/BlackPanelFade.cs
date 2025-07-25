using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackPanelFade : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Image panelImage;

    void Awake()
    {
        panelImage = GetComponent<Image>();

        // Dat mau den va alpha = 1
        if (panelImage != null)
        {
            panelImage.color = new Color(0, 0, 0, 1f);
        }

        // KHONG tat game object o day!
        // gameObject.SetActive(false); // Bo dong nay di
    }

    public void StartFadeOut()
    {
        Debug.Log("START DISAPPEAR");
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = 1f - (time / fadeDuration);
            if (panelImage != null)
                panelImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }

        if (panelImage != null)
            panelImage.color = new Color(0, 0, 0, 0f);

        Debug.Log("Fade xong, tat panel");
        gameObject.SetActive(false);
    }
}
