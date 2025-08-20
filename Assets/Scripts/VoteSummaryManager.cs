using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class VoteSummaryManager : MonoBehaviour
{
    public TMP_Text countdownText;
    public TMP_Text labelText;
    public float countdownSeconds = 5f;

    [Header("Canvas chuyển đổi")]
    public GameObject currentCanvas;
    public GameObject kickOutCanvas;

    [Header("Các AI trong game (9 AI - index 1–9)")]
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
            string name = sorted[0].Key;
            int voteCount = sorted[0].Value;

            if (sorted.Count > 1 && sorted[1].Value == voteCount)
            {
                Debug.Log("🟡 Hòa phiếu, không ai bị loại.");
                PlayerPrefs.SetString("EjectedResult", "Tie");
                PlayerPrefs.SetInt("VotedOutIndex", -1);
            }
            else if (name == "SKIP")
            {
                Debug.Log("😂 Mọi người skip vote.");
                PlayerPrefs.SetString("EjectedResult", "Skip");
                PlayerPrefs.SetInt("VotedOutIndex", -1);
            }
            else
            {
                // So sánh đúng tên người chơi (Human)
                string playerName = PlayerPrefs.GetString("PlayerName", "Human");
                int votedOutIndex = -1;

                for (int i = 0; i <= 9; i++)
                {
                    string savedName = PlayerPrefs.GetString($"Shuffle_Name_{i}", $"Unknown {i}");
                    if (savedName == name)
                    {
                        votedOutIndex = i;
                        break;
                    }
                }

                if (votedOutIndex == -1)
                {
                    Debug.LogError($"❌ Không tìm thấy index ứng với tên {name}!");
                    PlayerPrefs.SetInt("VotedOutIndex", -1);
                }
                else
                {
                    PlayerPrefs.SetInt("VotedOutIndex", votedOutIndex);
                    PlayerPrefs.SetString("EjectedResult", name);
                    Debug.Log($"❌ {name} bị loại với {voteCount} vote!");

                    if (name == playerName)
                    {
                        // Human bị loại
                        if (humanPlayer != null)
                        {
                            StartCoroutine(DestroyAfterDelay(humanPlayer, 5f));
                            Debug.Log("🕒 Người chơi sẽ bị hủy sau 5s.");
                        }
                    }
                    else
                    {
                        // AI bị loại
                        int aiArrayIndex = votedOutIndex;
                        if (aiArrayIndex >= 0 && aiArrayIndex < aiCharacters.Length)
                        {
                            if (aiCharacters[aiArrayIndex] != null)
                            {
                                StartCoroutine(DestroyAfterDelay(aiCharacters[aiArrayIndex], 5f));
                                Debug.Log($"🕒 AI {aiArrayIndex} sẽ bị hủy sau 5s.");
                            }
                        }
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

    IEnumerator DestroyAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            Destroy(target);
            Debug.Log($"💥 Object {target.name} đã bị Destroy.");
        }
    }
}
