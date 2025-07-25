using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteYesHandler : MonoBehaviour
{
    public Button yesButton;                     // Button YES trong panel
    public TMP_Text nameText;                    // TMP chứa tên nhân vật này
    public GameObject voteAllocatorTrigger;      // Kích hoạt tiến trình sau cùng

    public int characterIndex;
    public bool isPlayer;

    private bool hasVoted = false;

    void Start()
    {
        if (yesButton != null)
            yesButton.onClick.AddListener(OnClickYes);
    }

    void OnClickYes()
    {
        if (hasVoted) return;
        hasVoted = true;

        string characterName = nameText.text;
        VotingDataManager.Instance.AddVote(characterName, 1); // +1 vote

        VotingLockoutManager.LockAllButtonsExcept(this.gameObject);
        VotingUIHelper.Instance?.ShowHumanIVoted();

        // ✅ Luôn kích hoạt object, dù là Human hay AI
        if (voteAllocatorTrigger != null)
        {
            voteAllocatorTrigger.SetActive(true);
            Debug.Log($"✅ Trigger bật cho {characterName}");
        }
    }
}
