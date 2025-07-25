using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IVotedController : MonoBehaviour
{
    public float checkInterval = 0.2f;
    [Range(0f, 1f)] public float chanceToShow = 0.1f;
    public GameObject voteDistributorTrigger;

    private VotingButton[] votingButtons;
    private bool distributionTriggered = false;

    void Start()
    {
        votingButtons = Object.FindObjectsByType<VotingButton>(FindObjectsSortMode.None);
        StartCoroutine(HandleIVotedEffect());
    }

    IEnumerator HandleIVotedEffect()
    {
        while (!distributionTriggered)
        {
            yield return new WaitForSeconds(checkInterval);

            bool allVoted = true;

            foreach (VotingButton vb in votingButtons)
            {
                // Bo qua human
                if (vb.isPlayer) continue;

                if (vb.statusCheckObject != null && vb.statusCheckObject.activeSelf)
                {
                    if (!vb.imageIVoted.gameObject.activeSelf)
                    {
                        allVoted = false;

                        if (Random.value < chanceToShow)
                        {
                            vb.imageIVoted.gameObject.SetActive(true);
                        }
                    }
                }
            }


            if (allVoted)
            {
                if (voteDistributorTrigger != null)
                {
                    voteDistributorTrigger.SetActive(true);
                }
                distributionTriggered = true;
            }
        }
    }
}
