using UnityEngine;

public class ImpostorKillHandler : MonoBehaviour
{
    [Header("Kill Settings")]
    [SerializeField] private float killCooldown = 5f;      // Cooldown bình thường giữa các kill
    [SerializeField] private float uiBlockCooldown = 15f;  // Cooldown sau khi UI tắt hẳn
    [Range(0f, 1f)]
    [SerializeField] private float killChance = 0.05f;     // 5% tỉ lệ kill

    [Header("UI / Elements Blocking Kill")]
    [SerializeField] private GameObject[] blockElements;   // Nếu 1 cái nào active -> block kill

    private float lastKillTime = -Mathf.Infinity;
    private float uiBlockEndTime = -Mathf.Infinity; // Thời điểm kết thúc block UI

    private void Update()
    {
        if (IsAnyBlockElementActive())
        {
            // Nếu có element đang bật -> chưa bắt đầu đếm cooldown
            uiBlockEndTime = Mathf.Infinity;
        }
        else
        {
            // Nếu tất cả element tắt mà trước đó vẫn đang Infinity => bắt đầu block cooldown
            if (uiBlockEndTime == Mathf.Infinity)
            {
                uiBlockEndTime = Time.time + uiBlockCooldown;
                Debug.Log($"[UI BLOCK START] Bắt đầu block kill trong {uiBlockCooldown} giây.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CompareTag("Impostor")) return;
        if (!other.CompareTag("Crewmate")) return;

        // Nếu UI đang block hoặc cooldown UI chưa hết → return
        if (IsAnyBlockElementActive() || Time.time < uiBlockEndTime)
        {
            Debug.Log($"[BLOCKED] Không thể kill {other.name} vì UI đang bật hoặc cooldown UI chưa hết.");
            return;
        }

        // Cooldown kill bình thường
        if (Time.time - lastKillTime < killCooldown) return;

        // Tỉ lệ kill
        float chance = Random.Range(0f, 1f);
        if (chance <= killChance)
        {
            Debug.Log($"[KILL {killChance * 100}%] {gameObject.name} đã giết {other.name}");
            other.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[MISS] {gameObject.name} gặp {other.name} nhưng KHÔNG giết ({killChance * 100}% fail)");
        }

        lastKillTime = Time.time;
    }

    private bool IsAnyBlockElementActive()
    {
        foreach (GameObject go in blockElements)
        {
            if (go == null)
                continue; // Bỏ qua element bị destroyed hoặc chưa gán

            if (go.activeSelf)
                return true;
        }
        return false;
    }
}
