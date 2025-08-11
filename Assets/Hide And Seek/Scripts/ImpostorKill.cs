using UnityEngine;
using System.Collections;

public class ImpostorKill : MonoBehaviour
{
    [Header("Cài đặt")]
    public float killCooldown = 1f; // thời gian chờ sau khi kill
    private bool canKill = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu va chạm với Crewmate và có thể kill
        if (canKill && collision.CompareTag("Crewmate"))
        {
            // Ẩn Crewmate thay vì Destroy
            collision.gameObject.SetActive(false);

            // Bắt đầu cooldown
            StartCoroutine(KillCooldownRoutine());
        }
    }

    private IEnumerator KillCooldownRoutine()
    {
        canKill = false;
        yield return new WaitForSeconds(killCooldown);
        canKill = true;
    }
}
