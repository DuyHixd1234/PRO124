using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerImpostorController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button killButton;
    public TMP_Text killCooldownText;

    [Header("Canvas Group")]
    public CanvasGroup killButtonGroup;

    [Header("Cooldown")]
    public float killCooldown = 20f;
    private float cooldownTimer;
    private bool isCoolingDown = true; // ✅ Bắt đầu cooldown ngay khi scene load

    [Header("Detection")]
    public Transform detectZone;
    private AICrewmate targetCrew;

    [Header("Canvas")]
    public GameObject gameplayCanvas;

    void Start()
    {
        SetKillInteractable(false); // ✅ Tắt nút Kill từ đầu

        cooldownTimer = killCooldown;
        killCooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();

        killButton.onClick.AddListener(HandleKill);
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
}
