using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    private List<AIIdentifier> allAIs = new List<AIIdentifier>();
    private Dictionary<string, bool> previousStates = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        RefreshAIs();
    }

    void RefreshAIs()
    {
        allAIs.Clear();

        // ✅ Dùng API mới
        var identifiers = Object.FindObjectsByType<AIIdentifier>(FindObjectsSortMode.None);

        foreach (var ai in identifiers) // 🟢 đúng tên biến
        {
            if (!string.IsNullOrEmpty(ai.aiID))
            {
                allAIs.Add(ai);

                string key = $"AI_Eliminated_{ai.aiID}";
                if (PlayerPrefs.GetInt(key, 0) == 1)
                {
                    ai.gameObject.SetActive(false);
                    Debug.Log($"[ProgressManager] AI {ai.aiID} đã bị loại → SetActive(false)");
                }

                previousStates[ai.aiID] = ai.gameObject.activeSelf;
            }
        }
    }

    void Update()
    {
        foreach (var ai in allAIs)
        {
            if (ai == null || string.IsNullOrEmpty(ai.aiID)) continue;

            bool current = ai.gameObject.activeSelf;
            bool previous = previousStates.ContainsKey(ai.aiID) ? previousStates[ai.aiID] : true;

            if (previous && !current)
            {
                string key = $"AI_Eliminated_{ai.aiID}";
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();

                Debug.Log($"[ProgressManager] Ghi loại AI: {ai.aiID}");
            }

            previousStates[ai.aiID] = current;
        }
    }

    public bool IsAIEliminated(string aiID)
    {
        return PlayerPrefs.GetInt($"AI_Eliminated_{aiID}", 0) == 1;
    }
}
