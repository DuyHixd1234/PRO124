using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImpostorKill : MonoBehaviour
{
    [Header("Cài đặt")]
    public float killCooldown = 1f; // thời gian chờ sau khi kill
    private bool canKill = true;

    // Danh sách crewmate đang trong vùng trigger
    private List<GameObject> crewmatesInRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate") && !crewmatesInRange.Contains(collision.gameObject))
        {
            crewmatesInRange.Add(collision.gameObject);
            TryKill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate"))
        {
            crewmatesInRange.Remove(collision.gameObject);
        }
    }

    private void TryKill()
    {
        if (!canKill || crewmatesInRange.Count == 0) return;

        // Luôn kill 1 mục tiêu duy nhất
        GameObject target = crewmatesInRange[0];
        if (target != null && target.activeSelf)
        {
            target.SetActive(false);
            StartCoroutine(KillCooldownRoutine());
        }
    }

    private IEnumerator KillCooldownRoutine()
    {
        canKill = false;
        yield return new WaitForSeconds(killCooldown);
        canKill = true;

        // Sau cooldown, nếu vẫn còn crewmate trong vùng → kill tiếp
        if (crewmatesInRange.Count > 0)
        {
            TryKill();
        }
    }
}
