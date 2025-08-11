using UnityEngine;

public class TaskBehaviour : MonoBehaviour
{
    [Header("Âm thanh khi task bị phá")]
    public AudioSource audioSource;   // Audio Source (Awake = false)
    public AudioClip destroySound;    // Âm thanh khi task bị phá

    private bool hasTriggered = false; // Đảm bảo chỉ chạy 1 lần

    void Start()
    {
        // Không cần tìm AIImpostorHideSeek nữa vì đã bỏ giảm thời gian
    }

    void OnDisable()
    {
        HandleTaskDestroyed();
    }

    void OnDestroy()
    {
        HandleTaskDestroyed();
    }

    private void HandleTaskDestroyed()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        // Chỉ phát âm thanh khi task bị phá
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }
    }
}
