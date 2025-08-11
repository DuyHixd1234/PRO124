using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTMP : MonoBehaviour
{
    [Header("Thiết lập thời gian")]
    public float startTime = 10f; // Thời gian bắt đầu (giây)

    [Header("TMP Text hiển thị")]
    public TextMeshProUGUI countdownText;

    [Header("GameObject sẽ bật khi hết giờ")]
    public GameObject[] objectsToEnable;

    [Header("GameObject sẽ tắt khi hết giờ")]
    public GameObject[] objectsToDisable;

    private float currentTime;

    private void Start()
    {
        currentTime = startTime;

        if (countdownText == null)
        {
            Debug.LogError("[CountdownTMP] Bạn chưa gán TMP Text!");
            enabled = false;
            return;
        }

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            UpdateText();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        // Khi currentTime = 0
        currentTime = 0;
        UpdateText();

        // Set active theo danh sách
        foreach (var obj in objectsToEnable)
        {
            if (obj != null) obj.SetActive(true);
        }
        foreach (var obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void UpdateText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        countdownText.text = $"{minutes}:{seconds:00}";
    }
}
