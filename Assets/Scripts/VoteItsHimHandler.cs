using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteItsHimHandler : MonoBehaviour
{
    public Button itsHimButton;
    public TMP_Text nameText;                    // TMP chứa tên nhân vật của button này
    public GameObject voteAllocatorTrigger;
    public int characterIndex;
    public bool isPlayer;

    private bool hasVoted = false;

    void Start()
    {
        if (itsHimButton != null)
            itsHimButton.onClick.AddListener(OnClickItsHim);
    }

    void OnClickItsHim()
    {
        if (hasVoted) return;
        hasVoted = true;

        string characterName = nameText.text;
        VotingDataManager.Instance.AddVote(characterName, 10); // Dồn 10 vote
        VotingDataManager.Instance.MarkForceVote();
        VotingDataManager.Instance.SetForceVoteTarget(characterName);

        VotingLockoutManager.LockAllButtonsExcept(this.gameObject);
        VotingUIHelper.Instance?.ShowHumanIVoted();

        if (isPlayer && voteAllocatorTrigger != null)
            voteAllocatorTrigger.SetActive(true);
    }
}
