using UnityEngine;

public class ImpostorKillHandler : MonoBehaviour
{
    private float killCooldown = 5f;
    private float lastKillTime = -Mathf.Infinity;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.tag != "Impostor") return;
        if (!other.CompareTag("Crewmate")) return;
        if (Time.time - lastKillTime < killCooldown) return;

        float chance = Random.Range(0f, 1f);
        if (chance <= 0.5f) // 5% kill
        {
            Debug.Log($"[KILL 5%] {gameObject.name} đã giết {other.name}");
            other.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[MISS] {gameObject.name} gặp {other.name} nhưng KHÔNG giết (5% fail)");
        }

        lastKillTime = Time.time;
    }
}
