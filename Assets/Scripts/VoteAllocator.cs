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
    public TMP_Text skipVoteCountText;                // TMP hiển thị số vote SKIP (không dùng nữa)

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

                // Thêm vào danh sách AI nếu không phải Human (Human = index 0)
                if (vb.characterIndex != 0)
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

        // Delay trước khi bật Summary
        yield return new WaitForSeconds(4.5f); // gộp 1.5f + 3f

        if (voteSummaryManager != null)
            voteSummaryManager.SetActive(true);
    }
}
