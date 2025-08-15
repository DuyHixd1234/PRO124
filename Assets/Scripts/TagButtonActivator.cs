using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TagButtonActivator : MonoBehaviour
{
    [Header("Cài đặt Tag cần chạm")]
    public string targetTag = "???";

    [Header("Nút UI cần kích hoạt")]
    public Button targetButton;

    [Header("Object được bật khi click nút")]
    public GameObject targetObject;

    [Header("Thời gian bật Object (giây)")]
    public float activeDuration = 3f;

    [Header("Cooldown giữa 2 lần click (giây)")]
    public float cooldownTime = 5f;

    private bool canClick = true;

    void Start()
    {
        if (targetButton != null)
        {
            targetButton.interactable = false; // ban đầu không bấm được
            targetButton.onClick.AddListener(OnButtonClick);
        }

        if (targetObject != null)
            targetObject.SetActive(false);

        Debug.Log("[INIT] Script TagButtonActivator đã khởi tạo.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[TRIGGER ENTER] Va chạm với: {other.name}, tag = {other.tag}");

        if (other.CompareTag(targetTag))
        {
            Debug.Log("[TRIGGER ENTER] Đúng tag, bật nút!");
            if (targetButton != null)
                targetButton.interactable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"[TRIGGER EXIT] Rời khỏi: {other.name}, tag = {other.tag}");

        if (other.CompareTag(targetTag))
        {
            Debug.Log("[TRIGGER EXIT] Đúng tag, tắt nút!");
            if (targetButton != null)
                targetButton.interactable = false;
        }
    }

    void OnButtonClick()
    {
        Debug.Log($"[BUTTON CLICK] Nút được bấm! canClick = {canClick}");

        if (!canClick)
        {
            Debug.Log("[BUTTON CLICK] Nhưng đang cooldown, bỏ qua.");
            return;
        }

        StartCoroutine(ActivateObjectSequence());
    }

    private IEnumerator ActivateObjectSequence()
    {
        canClick = false;

        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Debug.Log($"[OBJECT] {targetObject.name} đã được bật!");
        }

        yield return new WaitForSeconds(activeDuration);

        if (targetObject != null)
        {
            targetObject.SetActive(false);
            Debug.Log($"[OBJECT] {targetObject.name} đã được tắt!");
        }

        Debug.Log($"[COOLDOWN] Bắt đầu chờ {cooldownTime} giây...");
        yield return new WaitForSeconds(cooldownTime);

        canClick = true;
        Debug.Log("[COOLDOWN] Hết thời gian chờ, có thể click lại.");
    }
}
