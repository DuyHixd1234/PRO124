using UnityEngine;
using System.Collections;

public class AIReportHandler : MonoBehaviour
{
    [Header("UI Report")]
    public GameObject deadbodyReportedCanvas;
    public GameObject discussCanvas;

    [Header("Delay đầu game")]
    public float initialDelay = 20f;

    private bool gameStarted = false;
    private bool canReport = false;

    void Start()
    {
        StartCoroutine(InitAfterDelay());
    }

    IEnumerator InitAfterDelay()
    {
        if (deadbodyReportedCanvas != null) deadbodyReportedCanvas.SetActive(false);
        if (discussCanvas != null) discussCanvas.SetActive(false);

        yield return new WaitForSeconds(initialDelay);

        gameStarted = true;
        canReport = true;
        Debug.Log($"[{gameObject.name}] ĐÃ SẴN SÀNG REPORT SAU DELAY (BỎ QUA TAG)");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameStarted || !canReport) return;

        if (other.CompareTag("Body"))
        {
            Debug.Log($"[REPORT] {gameObject.name} đã report xác!");
            StartCoroutine(HandleReportUI());
        }
    }

    private IEnumerator HandleReportUI()
    {
        if (deadbodyReportedCanvas != null)
            deadbodyReportedCanvas.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        if (deadbodyReportedCanvas != null)
            deadbodyReportedCanvas.SetActive(false);
        if (discussCanvas != null)
            discussCanvas.SetActive(true);
    }
}
