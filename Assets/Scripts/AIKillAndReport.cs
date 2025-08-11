using UnityEngine;
using System.Collections;

public class AIKillAndReport : MonoBehaviour
{
    private float lastKillTime = -Mathf.Infinity;
    private bool isImpostor;

    [Header("Kill Cooldown")]
    public float killCooldown = 5f;

    [Header("Delay đầu game")]
    public float initialDelay = 20f;

    private bool gameStarted = false;

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
        lastKillTime = Time.time - killCooldown; // Cho phép kill ngay
        Debug.Log($"[{gameObject.name}] ĐÃ SẴN SÀNG KILL SAU DELAY");
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
        if (Time.time - lastKillTime < killCooldown) return;

        float chance = Random.Range(0f, 1f);
        if (chance <= 0.05f) // 5% kill
        {
            Debug.Log($"[KILL 5%] {gameObject.name} đã giết {other.name}");
            other.gameObject.SetActive(false);
            lastKillTime = Time.time;
        }
        else
        {
            Debug.Log($"[MISS] {gameObject.name} gặp {other.name} nhưng KHÔNG giết (5% fail)");
            lastKillTime = Time.time; // reset để không spam check
        }
    }
}
    