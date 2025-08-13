using UnityEngine;
using System.Collections;

public class AIKillAndReport : MonoBehaviour
{
    private float lastKillTime = -Mathf.Infinity;
    private bool isImpostor;
    private bool gameStarted = false;

    [Header("Kill Cooldown Mặc Định")]
    public float killCooldown = 5f; // cooldown khi MISS
    public float killSuccessCooldown = 15f; // cooldown khi KILL

    [Header("Delay đầu game")]
    public float initialDelay = 20f;

    [Header("UI Canvases")]
    public GameObject[] uiCanvases = new GameObject[9];

    private bool uiWasOpenLastCheck = false;

    void Start()
    {
        isImpostor = CompareTag("Impostor");

        if (!isImpostor)
        {
            enabled = false;
            return;
        }

        Debug.Log($"[{gameObject.name}] Là Impostor – đợi {initialDelay}s để bắt đầu...");
        StartCoroutine(StartAfterDelay());
    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        gameStarted = true;
        lastKillTime = Time.time - killCooldown; // cho phép kill ngay (nếu UI không mở)
        Debug.Log($"[{gameObject.name}] ĐÃ SẴN SÀNG KILL SAU DELAY");
    }

    void Update()
    {
        if (!gameStarted) return;

        bool anyUIOpen = IsAnyCanvasOpen();

        // Nếu UI mới vừa mở → không cho kill
        if (anyUIOpen)
        {
            uiWasOpenLastCheck = true;
            return;
        }

        // Nếu UI vừa tắt sau khi mở → reset cooldown từ đầu
        if (!anyUIOpen && uiWasOpenLastCheck)
        {
            lastKillTime = Time.time; // reset, phải chờ cooldown mới được kill
            uiWasOpenLastCheck = false;
            Debug.Log($"[{gameObject.name}] UI vừa tắt → reset cooldown từ đầu");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameStarted) return;
        TryKill(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!gameStarted) return;
        TryKill(other);
    }

    void TryKill(Collider2D other)
    {
        if (!other.CompareTag("Crewmate")) return;
        if (IsAnyCanvasOpen()) return; // nếu UI đang bật thì không kill

        float timeSinceLastKill = Time.time - lastKillTime;
        if (timeSinceLastKill < killCooldown) return;

        float chance = Random.Range(0f, 1f);
        if (chance <= 0.05f) // 5% kill
        {
            Debug.Log($"[KILL 5%] {gameObject.name} đã giết {other.name}");
            other.gameObject.SetActive(false);
            lastKillTime = Time.time;
            killCooldown = killSuccessCooldown; // kill thành công → 15s cooldown
        }
        else
        {
            Debug.Log($"[MISS] {gameObject.name} gặp {other.name} nhưng KHÔNG giết (5% fail)");
            lastKillTime = Time.time;
            killCooldown = 5f; // miss → 5s cooldown
        }
    }

    bool IsAnyCanvasOpen()
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas != null && canvas.activeInHierarchy)
                return true;
        }
        return false;
    }
}
