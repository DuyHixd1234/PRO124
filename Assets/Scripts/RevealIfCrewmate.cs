using UnityEngine;

public class RevealIfCrewmate : MonoBehaviour
{
    [Header("Object cần bật nếu là Crewmate")]
    public GameObject targetObjectToReveal;

    private bool hasRevealed = false;

    void Update()
    {
        // Nếu đã bật trước đó → bỏ qua
        if (hasRevealed) return;

        // Nếu chưa gán object → cảnh báo rồi thoát
        if (targetObjectToReveal == null)
        {
            Debug.LogWarning($"[RevealIfCrewmate] {gameObject.name} chưa được gán object cần bật!");
            enabled = false; // Tắt script để tránh spam
            return;
        }

        // Nếu tag là Crewmate → bật object
        if (CompareTag("Crewmate"))
        {
            targetObjectToReveal.SetActive(true);
            hasRevealed = true;
            Debug.Log($"[RevealIfCrewmate] {gameObject.name} đã được gán tag 'Crewmate' → bật {targetObjectToReveal.name}");
        }
        // Nếu tag là Impostor → vô hiệu luôn
        else if (CompareTag("Impostor"))
        {
            Debug.Log($"[RevealIfCrewmate] {gameObject.name} là Impostor → không bật gì, tắt script.");
            enabled = false;
        }
        // Nếu vẫn là Untagged → tiếp tục đợi
    }
}
