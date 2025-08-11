using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class HSTask : MonoBehaviour
{
    [Header("Sliders")]
    public Slider firstSlider;
    public Slider whiteSlider;
    public Slider finalSlider;
    public TextMeshProUGUI timeTextFirst;
    public TextMeshProUGUI timeTextFinal;

    [Header("Role Check")]
    public GameObject roleObject; // Object để xác định role

    [Header("Role Canvases")]
    public GameObject impostorCanvas;
    public GameObject crewmateCanvas;

    [Header("Thời gian cho từng vai trò")]
    public float crewmateFirstTime = 100f;
    public float crewmateFinalTime = 30f;
    public float impostorFirstTime = 60f;
    public float impostorFinalTime = 30f;

    [Header("Giảm thời gian khi task bị phá")]
    public float minTimeReduce = 2f;
    public float maxTimeReduce = 5f;
    public Color flashColor = Color.white;
    public float flashDuration = 0.2f;

    [Header("Audio")]
    public AudioClip destroySound;

    private AudioSource parentAudioSource;
    private Image firstSliderFillImage;

    private bool isFinalSliderActive = false;
    private float currentTime;

    public float whiteSliderDelay = 0.5f;

    private void Start()
    {
        parentAudioSource = GetComponentInParent<AudioSource>();
        if (firstSlider != null && firstSlider.fillRect != null)
            firstSliderFillImage = firstSlider.fillRect.GetComponent<Image>();

        CheckRoleAndShowCanvas(); // Chỉ gọi 1 lần duy nhất

        SetInitialTimes();
    }


    private void Update()
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f) currentTime = 0f;
        }

        if (!isFinalSliderActive)
        {
            firstSlider.value = currentTime;

            if (whiteSlider != null)
                whiteSlider.value = Mathf.Lerp(whiteSlider.value, firstSlider.value, Time.deltaTime / whiteSliderDelay);

            if (timeTextFirst != null)
                timeTextFirst.text = FormatTime(currentTime);

            if (currentTime <= 0f && !finalSlider.gameObject.activeSelf)
            {
                ActivateFinalSlider();
            }
        }
        else
        {
            finalSlider.value = currentTime;
            if (timeTextFinal != null)
                timeTextFinal.text = FormatTime(currentTime);

            if (currentTime <= 0f)
            {
                StartCoroutine(HandleEndSequence());
            }
        }
    }

    private void CheckRoleAndShowCanvas()
    {
        // Xác định ngay từ đầu và giữ nguyên suốt game
        bool isImpostor = (roleObject != null && roleObject.activeSelf);

        if (isImpostor)
        {
            if (impostorCanvas != null) impostorCanvas.SetActive(true);
            if (crewmateCanvas != null) crewmateCanvas.SetActive(false);
        }
        else
        {
            if (impostorCanvas != null) impostorCanvas.SetActive(false);
            if (crewmateCanvas != null) crewmateCanvas.SetActive(true);
        }

        // Sau khi set, không cần gọi lại hàm này nữa
        // vì vai trò đã cố định ngay từ Start
    }



    private void SetInitialTimes()
    {
        bool isImpostor = (roleObject != null);

        if (!isImpostor)
        {
            firstSlider.maxValue = crewmateFirstTime;
            whiteSlider.maxValue = crewmateFirstTime;
            finalSlider.maxValue = crewmateFinalTime;
            currentTime = crewmateFirstTime;
        }
        else
        {
            firstSlider.maxValue = impostorFirstTime;
            whiteSlider.maxValue = impostorFirstTime;
            finalSlider.maxValue = impostorFinalTime;
            currentTime = impostorFirstTime;
        }

        firstSlider.value = currentTime;
        whiteSlider.value = currentTime;
        finalSlider.gameObject.SetActive(false);
    }

    public void NotifyTaskDestroyed()
    {
        if (isFinalSliderActive) return;

        float reduceAmount = Random.Range(minTimeReduce, maxTimeReduce);
        currentTime = Mathf.Max(0, currentTime - reduceAmount);

        if (firstSliderFillImage != null)
            StartCoroutine(FlashSliderColor());

        if (parentAudioSource != null && destroySound != null)
            parentAudioSource.PlayOneShot(destroySound);
    }

    private IEnumerator FlashSliderColor()
    {
        Color originalColor = firstSliderFillImage.color;
        firstSliderFillImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        firstSliderFillImage.color = originalColor;
    }

    private void ActivateFinalSlider()
    {
        isFinalSliderActive = true;
        finalSlider.gameObject.SetActive(true);

        bool isImpostor = (roleObject != null);
        currentTime = isImpostor ? impostorFinalTime : crewmateFinalTime;

        finalSlider.maxValue = currentTime;
        finalSlider.value = currentTime;

        foreach (Transform child in transform)
        {
            if (child != firstSlider.transform && child != whiteSlider.transform && child != finalSlider.transform)
                child.gameObject.SetActive(false);
        }
    }

    private IEnumerator HandleEndSequence()
    {
        bool isImpostor = (roleObject != null);

        yield return new WaitForSeconds(1f);

        if (isImpostor)
            SceneManager.LoadScene("HSLose");
        else
            SceneManager.LoadScene("HSWin");
    }

    private void OnTransformChildrenChanged()
    {
        NotifyTaskDestroyed();
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes}:{seconds:00}";
    }
}
