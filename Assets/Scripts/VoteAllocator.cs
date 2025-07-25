using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VoteAllocator : MonoBehaviour
{
    private VotingButton[] votingButtons;
    private List<VotingButton> aiButtons = new List<VotingButton>();
    private List<string> voteTargets = new List<string>();
    private Dictionary<string, int> voteResults = new Dictionary<string, int>();

    public GameObject skipButtonObject;               // Button SKIP
    public GameObject voteSummaryManager;             // Hiện sau khi xử lý xong
    public TMP_Text skipVoteCountText;                // TMP hiển thị số vote SKIP
    string topVoted = "";
    int topVote = -1;
    bool tie = false;
    void OnEnable()
    {
        StartCoroutine(AllocateVotes());
    }

    IEnumerator AllocateVotes()
    {
        votingButtons = Object.FindObjectsByType<VotingButton>(FindObjectsSortMode.None);
        aiButtons.Clear();
        voteTargets.Clear();
        voteResults.Clear();

        foreach (var vb in votingButtons)
        {
            if (vb.statusCheckObject != null && vb.statusCheckObject.activeSelf)
            {
                string name = vb.nameText.text;
                voteTargets.Add(name);
                voteResults[name] = 0;

                if (!vb.isPlayer)
                    aiButtons.Add(vb);
            }
        }

        if (skipButtonObject != null)
        {
            voteTargets.Add("SKIP");
            voteResults["SKIP"] = 0;
        }

        if (VotingDataManager.Instance.IsForceVoteUsed())
        {
            string target = VotingDataManager.Instance.GetForceVoteTarget();
            if (!string.IsNullOrEmpty(target))
            {
                voteResults[target] += 10;
            }
        }
        else
        {
            foreach (var ai in aiButtons)
            {
                int r = Random.Range(0, voteTargets.Count);
                string chosen = voteTargets[r];
                voteResults[chosen] += 1;
            }
        }

        // Delay trước khi hiển thị số vote
        yield return new WaitForSeconds(1.5f);

        // Hiển thị vote count
        foreach (var vb in votingButtons)
        {
            string name = vb.nameText.text;
            int count = voteResults.ContainsKey(name) ? voteResults[name] : 0;

            if (vb.voteCountTMP != null)
            {
                vb.voteCountTMP.text = count.ToString();
                vb.voteCountTMP.gameObject.SetActive(true);
            }
        }

        if (skipVoteCountText != null && voteResults.ContainsKey("SKIP"))
        {
            skipVoteCountText.text = voteResults["SKIP"].ToString();
            skipVoteCountText.gameObject.SetActive(true);
        }

        // Delay thêm trước khi bật Summary
        yield return new WaitForSeconds(3f);

        if (voteSummaryManager != null)
            voteSummaryManager.SetActive(true);
    }
}
    