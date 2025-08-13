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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            if (targetButton != null)
                targetButton.interactable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            if (targetButton != null)
                targetButton.interactable = false;
        }
    }

    void OnButtonClick()
    {
        if (!canClick) return;

        StartCoroutine(ActivateObjectSequence());
    }

    private IEnumerator ActivateObjectSequence()
    {
        canClick = false;

        if (targetObject != null)
            targetObject.SetActive(true);

        yield return new WaitForSeconds(activeDuration);

        if (targetObject != null)
            targetObject.SetActive(false);

        yield return new WaitForSeconds(cooldownTime);

        canClick = true;
    }
}
