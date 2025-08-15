using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KillButtonController : MonoBehaviour
{
    [Header("Button & UI")]
    public Button killButton;
    public TMP_Text cooldownText;

    [Header("Kill Effect Object")]
    public GameObject killObject; // object ẩn, bật 0.3s khi click

    [Header("Range Detector (Trigger Collider)")]
    public Collider2D killRange; // collider trigger để check crewmate

    [Header("Cooldown Settings")]
    public float killCooldown = 15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip killSound;

    [Header("UI Block Elements (9 Canvas Objects)")]
    public GameObject[] blockElements;

    [Header("Debug & Setup")]
    public bool autoAddClickListener = true;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    private bool wasAnyUIActive = false;
    private bool hasCrewmateInRange = false;

    void Start()
    {
        if (killButton != null && autoAddClickListener)
            killButton.onClick.AddListener(OnKillClick);

        if (killRange != null)
            killRange.isTrigger = true;

        StartCooldown();
    }

    void Update()
    {
        // 1️⃣ Luôn check crewmate trong vùng mỗi frame
        hasCrewmateInRange = CheckCrewmateInRange();

        // 2️⃣ Quản lý cooldown và UI block
        bool anyUIActive = IsAnyBlockElementActive();
        if (anyUIActive && !wasAnyUIActive)
            StartCooldown();

        wasAnyUIActive = anyUIActive;

        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownText();

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                cooldownText.text = "";
            }
        }

        // 3️⃣ Set trạng thái nút
        killButton.interactable = !isCoolingDown && hasCrewmateInRange;
    }

    bool CheckCrewmateInRange()
    {
        if (killRange == null) return false;

        Collider2D[] hits = new Collider2D[10];
        int count = Physics2D.OverlapCollider(killRange, new ContactFilter2D().NoFilter(), hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] != null && hits[i].CompareTag("Crewmate") && hits[i].gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void OnKillClick()
    {
        if (isCoolingDown || !hasCrewmateInRange) return;

        if (audioSource != null && killSound != null)
            audioSource.PlayOneShot(killSound);

        if (killObject != null)
            StartCoroutine(ActivateKillObject());

        StartCooldown();
    }

    IEnumerator ActivateKillObject()
    {
        killObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        killObject.SetActive(false);
    }

    void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = killCooldown;
        UpdateCooldownText();
        killButton.interactable = false;
    }

    void UpdateCooldownText()
    {
        if (cooldownText != null)
            cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
    }

    bool IsAnyBlockElementActive()
    {
        foreach (GameObject go in blockElements)
        {
            if (go != null && go.activeSelf) return true;
        }
        return false;
    }
}
