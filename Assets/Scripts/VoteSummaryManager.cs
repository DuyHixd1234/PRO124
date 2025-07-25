using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class VoteSummaryManager : MonoBehaviour
{
    public TMP_Text countdownText;
    public TMP_Text labelText; // Proceeding in
    public float countdownSeconds = 5f;

    [Header("Canvas chuyển đổi")]
    public GameObject currentCanvas;   // canvas hiện tại (sẽ bị ẩn)
    public GameObject kickOutCanvas;   // canvas mới (Canvas kick-out)

    [Header("Các AI trong game (9 AI)")]
    public GameObject[] aiCharacters = new GameObject[9];

    [Header("Human player object")]
    public GameObject humanPlayer;

    void OnEnable()
    {
        StartCoroutine(CountdownAndHandleResult());
    }

    IEnumerator CountdownAndHandleResult()
    {
        if (labelText != null) labelText.gameObject.SetActive(true);
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        float time = countdownSeconds;
        while (time > 0)
        {
            countdownText.text = Mathf.Ceil(time).ToString();
            yield return new WaitForSeconds(1f);
            time -= 1f;
        }

        countdownText.text = "0";

        var results = VotingDataManager.Instance != null ? VotingDataManager.Instance.voteCounts : null;

        if (results == null || results.Count == 0)
        {
            Debug.Log("❌ Không có dữ liệu vote.");
            PlayerPrefs.SetString("EjectedResult", "None");
            PlayerPrefs.SetInt("VotedOutIndex", -1);
        }
        else
        {
            var sorted = results.OrderByDescending(kv => kv.Value).ToList();
            string topName = sorted[0].Key;
            int topVote = sorted[0].Value;

            if (sorted.Count > 1 && sorted[1].Value == topVote)
            {
                Debug.Log("🟡 Hòa phiếu, không ai bị loại.");
                PlayerPrefs.SetString("EjectedResult", "Tie");
                PlayerPrefs.SetInt("VotedOutIndex", -1);
            }
            else if (topName == "SKIP")
            {
                Debug.Log("😂 Mọi người skip vote.");
                PlayerPrefs.SetString("EjectedResult", "Skip");
                PlayerPrefs.SetInt("VotedOutIndex", -1);
            }
            else
            {
                Debug.Log($"❌ {topName} bị loại với {topVote} vote!");
                PlayerPrefs.SetString("EjectedResult", topName);

                string humanName = PlayerData.Instance != null ? PlayerData.Instance.playerName : "You";

                if (topName == humanName)
                {
                    Debug.Log($"❌ Human ({humanName}) bị loại với {topVote} vote!");

                    // Ẩn Human nếu object tồn tại
                    if (humanPlayer != null)
                    {
                        humanPlayer.SetActive(false);
                        Debug.Log("🚫 Human player đã bị ẩn.");
                    }

                    // Human sẽ dùng index 10
                    PlayerPrefs.SetInt("VotedOutIndex", 10);
                }
                else
                {
                    Debug.Log($"✅ Human ({humanName}) an toàn! Người bị loại là: {topName} ({topVote} vote)");

                    // Tìm AI index từ Shuffle_Name_0~8
                    int foundIndex = -1;
                    for (int i = 0; i < 9; i++)
                    {
                        string savedName = PlayerPrefs.GetString($"Shuffle_Name_{i}", $"Unknown {i}");
                        if (savedName == topName)
                        {
                            foundIndex = i;
                            break;
                        }
                    }

                    if (foundIndex != -1)
                    {
                        PlayerPrefs.SetInt("VotedOutIndex", foundIndex);
                        Debug.Log($"🔍 VotedOutIndex = {foundIndex} lưu thành công.");

                        if (aiCharacters != null && foundIndex < aiCharacters.Length)
                        {
                            if (aiCharacters[foundIndex] != null)
                            {
                                aiCharacters[foundIndex].SetActive(false);
                                Debug.Log($"🧍‍♂️ AI {foundIndex} đã bị ẩn.");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"❌ Không tìm thấy index AI cho {topName}!");    
                        PlayerPrefs.SetInt("VotedOutIndex", -1);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (currentCanvas != null)
            currentCanvas.SetActive(false);

        if (kickOutCanvas != null)
            kickOutCanvas.SetActive(true);

        Debug.Log("✅ Chuyển sang Canvas kick-out.");
    }

}
