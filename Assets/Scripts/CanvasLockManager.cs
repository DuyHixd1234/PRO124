using UnityEngine;

public class CanvasLockManager : MonoBehaviour
{
    [Header("Canvas cần quản lý")]
    public GameObject votingResultCanvas;
    public GameObject discussCanvas;
    public GameObject deadbodyReportedCanvas;

    void Update()
    {
        if (votingResultCanvas != null && votingResultCanvas.activeSelf)
        {
            // Nếu voting result đang bật → tắt 2 canvas kia
            if (discussCanvas != null && discussCanvas.activeSelf)
            {
                Debug.LogWarning("[CanvasLock] Đang trong VotingResult – TẮT discussCanvas");
                discussCanvas.SetActive(false);
            }

            if (deadbodyReportedCanvas != null && deadbodyReportedCanvas.activeSelf)
            {
                Debug.LogWarning("[CanvasLock] Đang trong VotingResult – TẮT deadbodyReportedCanvas");
                deadbodyReportedCanvas.SetActive(false);
            }
        }
    }
}
