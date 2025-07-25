using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TaskGroup : MonoBehaviour
{
    [Header("Progress Settings")]
    public Slider progressSlider;             // Slider thanh tien trinh

    [Header("Fade Settings")]
    public CanvasGroup blackPanelCanvasGroup; // CanvasGroup panel den
    public float fadeDuration = 2f;

    private int totalTasks;
    private int completedTasks = 0;
    private bool winStarted = false;

    void Start()
    {
        totalTasks = transform.childCount;
        completedTasks = 0;

        if (blackPanelCanvasGroup != null)
        {
            blackPanelCanvasGroup.alpha = 0f;
            blackPanelCanvasGroup.gameObject.SetActive(false);
        }

        UpdateProgress();
    }

    public void NotifyTaskCompleted()
    {
        completedTasks++;
        UpdateProgress();

        if (completedTasks >= totalTasks && !winStarted)
        {
            winStarted = true;
            StartCoroutine(HandleWinSequence());
        }
    }

    void UpdateProgress()
    {
        if (progressSlider != null)
        {
            float ratio = totalTasks == 0 ? 1f : (float)completedTasks / totalTasks;
            progressSlider.value = ratio;
        }
    }

    IEnumerator HandleWinSequence()
    {
        yield return new WaitForSeconds(2f);

        if (blackPanelCanvasGroup != null)
        {
            blackPanelCanvasGroup.gameObject.SetActive(true);

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                blackPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
        }

        SceneManager.LoadScene("Win");
    }
}
