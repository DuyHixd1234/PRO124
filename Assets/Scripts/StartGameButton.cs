using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartGameButton : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text gameStartText;
    public TMP_Text countdownText;
    public GameObject fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Scene")]
    public string nextSceneName = "Loading";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip startSound;

    public void OnClickStart()
    {
        GetComponent<Button>().interactable = false;

        Debug.Log("CLICKED START - Playing sound...");

        if (audioSource != null && startSound != null)
            audioSource.PlayOneShot(startSound);
        else
            Debug.LogWarning("AudioSource hoặc StartSound bị null!");

        StartCoroutine(StartCountdownAndFade());
    }


    IEnumerator StartCountdownAndFade()
    {
        gameStartText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "";
        StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToBlack()
    {
        CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = fadePanel.AddComponent<CanvasGroup>();

        fadePanel.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
