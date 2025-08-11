using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AutoVoteManager : MonoBehaviour
{
    [Header("AI Characters (9 max)")]
    public GameObject[] aiCharacters;

    [Header("All Vote Buttons (10 players + 1 skip = 11)")]
    public Button[] voteButtons;

    [Header("Optional: Visual indicators for dead characters")]
    public GameObject[] deadIcons;

    [Header("Human vote buttons")]
    public Button humanYesButton;
    public Button humanItsHimButton;

    [Header("Target button selected by Human (set from UI)")]
    public Button humanSelectedButton;

    private Dictionary<Button, int> voteData = new Dictionary<Button, int>();
    private Dictionary<Button, int> buttonToIndexMap = new Dictionary<Button, int>();
    private int totalAIVotes = 9;
    private bool votingDone = false;

    void Awake()
    {
        Debug.Log("[AutoVote] Awake()");
        buttonToIndexMap.Clear();
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (voteButtons[i] != null)
            {
                buttonToIndexMap[voteButtons[i]] = i;
                Debug.Log($"[AutoVote] Button mapped: {voteButtons[i].name} → Index {i}");
            }
            else
            {
                Debug.LogWarning($"[AutoVote] voteButtons[{i}] is NULL!");
            }
        }
    }

    void OnEnable()
    {
        Debug.Log("[AutoVote] OnEnable()");
        votingDone = false;

        if (humanYesButton != null)
            humanYesButton.onClick.AddListener(() => RegisterHumanYesVote(humanSelectedButton));
        else
            Debug.LogWarning("[AutoVote] humanYesButton is NULL!");

        if (humanItsHimButton != null)
            humanItsHimButton.onClick.AddListener(() => RegisterHumanItsHimVote(humanSelectedButton));
        else
            Debug.LogWarning("[AutoVote] humanItsHimButton is NULL!");

        ProcessVoting();
    }

    void ProcessVoting()
    {
        if (votingDone) return;

        voteData.Clear();
        VotingDataManager.Instance.ClearVotes();

        int aliveAI = aiCharacters.Count(ai => ai != null && ai.activeInHierarchy);
        totalAIVotes = aliveAI;
        Debug.Log($"[AutoVote] Total alive AI: {aliveAI}");

        List<Button> validButtons = new List<Button>();
        for (int i = 0; i < voteButtons.Length; i++)
        {
            if (voteButtons[i] == null)
            {
                Debug.LogWarning($"[AutoVote] voteButtons[{i}] is NULL!");
                continue;
            }

            bool isDead = false;
            if (i < deadIcons.Length && deadIcons[i] != null)
                isDead = deadIcons[i].activeInHierarchy;

            // Với Skip (index 10) thì luôn hợp lệ
            if (!isDead || i == 10)
            {
                validButtons.Add(voteButtons[i]);
                Debug.Log($"[AutoVote] Valid target: {voteButtons[i].name}");
            }
            else
            {
                Debug.Log($"[AutoVote] Skipped button (dead): {voteButtons[i].name}");
            }
        }

        if (validButtons.Count == 0)
        {
            Debug.LogWarning("[AutoVote] Không có button nào để vote!");
            return;
        }

        for (int i = 0; i < totalAIVotes; i++)
        {
            int randIndex = Random.Range(0, validButtons.Count);
            Button chosen = validButtons[randIndex];

            if (!voteData.ContainsKey(chosen))
                voteData[chosen] = 0;
            voteData[chosen]++;
        }

        foreach (var kvp in voteData)
        {
            Button btn = kvp.Key;
            int count = kvp.Value;

            if (!buttonToIndexMap.ContainsKey(btn))
            {
                Debug.LogWarning($"[AutoVote] Không tìm thấy index của button {btn.name}");
                continue;
            }

            int index = buttonToIndexMap[btn];
            string trueName = index == 10
                ? "Skip"
                : PlayerPrefs.GetString($"Shuffle_Name_{index}", $"Unknown_{index}");

            Debug.Log($"[AutoVote] AI vote: {trueName} nhận {count} vote");
            VotingDataManager.Instance.AddVote(trueName, count);
        }

        votingDone = true;
    }


    public void RegisterHumanYesVote(Button votedButton)
    {
        Debug.Log("[HumanVote] YES vote clicked");

        if (votedButton == null)
        {
            Debug.LogWarning("[HumanVote] Không chọn nhân vật nào để vote!");
            return;
        }

        if (!voteData.ContainsKey(votedButton))
            voteData[votedButton] = 0;

        voteData[votedButton]++;

        if (!buttonToIndexMap.ContainsKey(votedButton))
        {
            Debug.LogError("[HumanVote] Không tìm thấy index của button được chọn!");
            return;
        }

        int index = buttonToIndexMap[votedButton];
        string trueName = PlayerPrefs.GetString($"Shuffle_Name_{index}", $"Unknown_{index}");

        VotingDataManager.Instance.AddVote(trueName, 1);
        Debug.Log($"[HumanVote] Human voted YES cho: {trueName}, tổng cộng = {voteData[votedButton]}");
    }

    public void RegisterHumanItsHimVote(Button votedButton)
    {
        Debug.Log("[HumanVote] IT'S HIM vote clicked");

        if (votedButton == null)
        {
            Debug.LogWarning("[HumanVote] Không chọn nhân vật nào để vote!");
            return;
        }

        List<Button> validTargets = new List<Button>();
        foreach (var b in voteButtons)
        {
            if (b == null || b == votedButton) continue;

            int index = System.Array.IndexOf(voteButtons, b);
            bool isDead = index >= 0 && index < deadIcons.Length && deadIcons[index] != null && deadIcons[index].activeInHierarchy;

            if (!isDead)
                validTargets.Add(b);
        }

        int voteCount = validTargets.Count + 1;

        voteData.Clear();
        voteData[votedButton] = voteCount;
        VotingDataManager.Instance.ClearVotes();

        if (!buttonToIndexMap.ContainsKey(votedButton))
        {
            Debug.LogError("[HumanVote] Không tìm thấy index của button được chọn!");
            return;
        }

        int indexVoted = buttonToIndexMap[votedButton];
        string trueName = PlayerPrefs.GetString($"Shuffle_Name_{indexVoted}", $"Unknown_{indexVoted}");

        VotingDataManager.Instance.AddVote(trueName, voteCount);
        Debug.Log($"[HumanVote] Human voted IT'S HIM cho: {trueName}, nhận {voteCount} vote");
    }

    public void ResetVoting()
    {
        Debug.Log("[AutoVote] ResetVoting()");
        voteData.Clear();
        votingDone = false;
    }

    public Dictionary<Button, int> GetVoteData()
    {
        return voteData;
    }

    public Dictionary<Button, int> GetButtonToIndexMap()
    {
        return buttonToIndexMap;
    }
}
