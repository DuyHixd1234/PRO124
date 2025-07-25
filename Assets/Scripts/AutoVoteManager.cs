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

            bool isDead = deadIcons.Length > i && deadIcons[i] != null && deadIcons[i].activeInHierarchy;

            if (!isDead)
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
            Debug.Log($"[AutoVote] AI vote: {kvp.Key.name} nhận {kvp.Value} vote");
            VotingDataManager.Instance.AddVote(kvp.Key.name, kvp.Value);
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
        VotingDataManager.Instance.AddVote(votedButton.name, 1);

        Debug.Log($"[HumanVote] Human voted YES cho: {votedButton.name}, tổng cộng = {voteData[votedButton]}");
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
        VotingDataManager.Instance.AddVote(votedButton.name, voteCount);

        Debug.Log($"[HumanVote] Human voted IT'S HIM cho: {votedButton.name}, nhận {voteCount} vote");
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
