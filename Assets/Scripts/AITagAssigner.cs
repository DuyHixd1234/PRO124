using UnityEngine;
using System.Collections;

public class AITagAssigner : MonoBehaviour
{
    [Header("Gán 9 AI GameObjects theo thứ tự đúng")]
    public GameObject[] aiObjects = new GameObject[9];

    [Header("4 GameObject của Player - chỉ 1 cái được bật")]
    public GameObject redPlayer;
    public GameObject bluePlayer;
    public GameObject purplePlayer;
    public GameObject yellowPlayer;

    [Header("Canvas báo cáo và thảo luận")]
    public GameObject deadbodyReportedCanvas;
    public GameObject discussCanvas;

    private string playerTag;
    private bool reportTriggered = false;

    void Start()
    {
        AssignTagsFromShuffle();
    }

    void AssignTagsFromShuffle()
    {
        // Gán tag cho AI (index 1 -> 9)
        for (int i = 0; i < aiObjects.Length; i++)
        {
            string nameKey = $"Shuffle_Name_{i + 1}";
            string roleKey = $"Shuffle_Role_{i + 1}";

            if (!PlayerPrefs.HasKey(nameKey) || !PlayerPrefs.HasKey(roleKey))
            {
                Debug.LogWarning($"[TagAssigner] Không tìm thấy dữ liệu tại index {i}.");
                continue;
            }

            int roleValue = PlayerPrefs.GetInt(roleKey);
            string tagToSet = roleValue == 1 ? "Impostor" : "Crewmate";

            if (aiObjects[i] != null)
            {
                aiObjects[i].tag = tagToSet;
                Debug.Log($"[TagAssigner] Gán tag '{tagToSet}' cho {aiObjects[i].name}");
            }
            else
            {
                Debug.LogWarning($"[TagAssigner] AI GameObject tại index {i} đang để trống!");
            }
        }

        // Gán tag cho player (index 0)
        int playerRole = PlayerPrefs.GetInt("Shuffle_Role_0", 0);
        playerTag = playerRole == 1 ? "Impostor" : "Crewmate";

        if (redPlayer.activeSelf) redPlayer.tag = playerTag;
        if (bluePlayer.activeSelf) bluePlayer.tag = playerTag;
        if (purplePlayer.activeSelf) purplePlayer.tag = playerTag;
        if (yellowPlayer.activeSelf) yellowPlayer.tag = playerTag;

        Debug.Log($"[TagAssigner] Gán tag '{playerTag}' cho Player (game object đang active)");
    }

    // ================= CODE TẠM: Xử lý kill bằng va chạm =================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("Impostor") && other.CompareTag("Crewmate"))
        {
            if (gameObject.name.Contains("Player"))
            {
                Debug.Log("[Kill] Player là Impostor - không dùng va chạm để kill.");
                return;
            }

            Debug.Log($"[Kill] {gameObject.name} kill {other.name} qua va chạm!");
            other.gameObject.SetActive(false);
        }

        // ================= Deadbody Report =================
        if (!reportTriggered && CompareTag("Crewmate") && other.CompareTag("Body"))
        {
            reportTriggered = true;
            StartCoroutine(HandleDeadbodyReport());
        }
    }

    IEnumerator HandleDeadbodyReport()
    {
        Debug.Log("[Report] Dead body detected! Hiện canvas báo cáo...");

        // Tắt AI hoặc dừng chuyển động (nếu bạn có AIController, hãy dùng .enabled = false hoặc .StopMoving())
        foreach (GameObject ai in aiObjects)
        {
            if (ai != null)
                ai.SetActive(false); // hoặc ai.GetComponent<AIController>().enabled = false;
        }

        if (deadbodyReportedCanvas != null)
            deadbodyReportedCanvas.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        if (deadbodyReportedCanvas != null)
            deadbodyReportedCanvas.SetActive(false);

        if (discussCanvas != null)
            discussCanvas.SetActive(true);

        Debug.Log("[Report] Chuyển sang canvas Discuss.");
    }
}
