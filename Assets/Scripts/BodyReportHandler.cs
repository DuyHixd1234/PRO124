using UnityEngine;
using System.Collections;

public class BodyReportHandler : MonoBehaviour
{
    [Header("Canvas liên quan")]
    public GameObject deadbodyCanvas; // Canvas xác chết

    [Header("Cài đặt")]
    public float deadbodyDisplayTime = 2.5f;

    private bool isReported = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReported) return; // tránh report nhiều lần
        if (collision.CompareTag("Crewmate"))
        {
            isReported = true;
            StartCoroutine(ReportSequence());
        }
    }

    private IEnumerator ReportSequence()
    {
        // 1. Dừng toàn bộ nhân vật
        StopAllCharacters();

        // 2. Teleport toàn bộ nhân vật về spawn point
        TeleportAllCharactersToSpawn();

        // 3. Hiển thị Deadbody Canvas
        if (deadbodyCanvas != null)
        {
            deadbodyCanvas.SetActive(true);
            yield return new WaitForSeconds(deadbodyDisplayTime);
            Destroy(deadbodyCanvas);
        }

        // 4. Quay lại gameplay
        ResumeAllCharacters();
    }

    private void StopAllCharacters()
    {
        // TODO: gọi hàm dừng AI + Human di chuyển
    }

    private void TeleportAllCharactersToSpawn()
    {
        // TODO: gọi hàm dịch chuyển tất cả về điểm spawn
    }

    private void ResumeAllCharacters()
    {
        // TODO: cho AI + Human di chuyển lại
    }
}
