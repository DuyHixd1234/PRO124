using System.Collections.Generic;
using UnityEngine;

public class VotingDataManager : MonoBehaviour
{
    public static VotingDataManager Instance;

    public Dictionary<string, int> voteCounts = new Dictionary<string, int>();
    private bool forceVoteUsed = false;
    private bool forceVote = false;
    private string forceVoteTarget = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // giữ sống giữa các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddVote(string name, int amount)
    {
        if (!voteCounts.ContainsKey(name))
            voteCounts[name] = 0;

        voteCounts[name] += amount;

        Debug.Log($"Vote added: {name} +{amount} (Total: {voteCounts[name]})");
    }

    public void MarkForceVote()
    {
        forceVoteUsed = true;
    }

    public bool IsForceVoteUsed()
    {
        return forceVoteUsed;
    }

    public void SetForceVoteTarget(string targetName)
    {
        forceVoteTarget = targetName;
    }

    public string GetForceVoteTarget()
    {
        return forceVoteTarget;
    }

    public bool IsForceVote()
    {
        return forceVoteUsed && !string.IsNullOrEmpty(forceVoteTarget);
    }

    public void ClearVotes()
    {
        voteCounts.Clear();
        forceVoteUsed = false;
        forceVoteTarget = "";
    }
}
