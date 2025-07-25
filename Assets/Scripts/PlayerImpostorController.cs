using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerImpostorController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button killButton;
    public TMP_Text killCooldownText;
    public Button reportButton;

    [Header("Canvas Group")]
    public CanvasGroup killButtonGroup;
    public CanvasGroup reportButtonGroup;

    [Header("Cooldown")]
    public float killCooldown = 20f;
    private float cooldownTimer;
    private bool isCoolingDown = true; // ✅ Bắt đầu cooldown ngay khi scene load

    [Header("Detection")]
    public Transform detectZone;
    private AICrewmate targetCrew;
    private GameObject nearbyBody;

    [Header("Canvas")]
    public GameObject gameplayCanvas;
    public GameObject deadBodyReportedCanvas;
    public GameObject discussCanvas;

    void Start()
    {
        SetKillInteractable(false); // ✅ Tắt nút Kill từ đầu
        SetReportInteractable(false);

        cooldownTimer = killCooldown;
        killCooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();

        killButton.onClick.AddListener(HandleKill);
        reportButton.onClick.AddListener(HandleReport);
    }

    void Update()
    {
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            killCooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                killCooldownText.text = "";

                // Không bật killButton ở đây — chờ chạm tag Crewmate
            }
        }
    }

    void SetKillInteractable(bool state)
    {
        killButton.interactable = state;
        killButtonGroup.alpha = state ? 1f : 0.4f;
    }

    void SetReportInteractable(bool state)
    {
        reportButton.interactable = state;
        reportButtonGroup.alpha = state ? 1f : 0.4f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate") && !isCoolingDown)
        {
            targetCrew = collision.GetComponent<AICrewmate>();
            if (targetCrew != null && targetCrew.gameObject.activeSelf)
            {
                SetKillInteractable(true);
            }
        }

        if (collision.CompareTag("Body"))
        {
            nearbyBody = collision.gameObject;
            SetReportInteractable(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate"))
        {
            if (targetCrew != null && collision.gameObject == targetCrew.gameObject)
            {
                targetCrew = null;
                SetKillInteractable(false);
            }
        }

        if (collision.CompareTag("Body"))
        {
            nearbyBody = null;
            SetReportInteractable(false);
        }
    }

    void HandleKill()
    {
        if (targetCrew == null) return;

        targetCrew.Kill();

        SetKillInteractable(false);
        isCoolingDown = true;
        cooldownTimer = killCooldown;
        killCooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
    }

    void HandleReport()
    {
        SetReportInteractable(false);
        gameplayCanvas.SetActive(false);
        StartCoroutine(ReportSequence());
    }

    IEnumerator ReportSequence()
    {
        deadBodyReportedCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        deadBodyReportedCanvas.SetActive(false);
        discussCanvas.SetActive(true);
    }
}
