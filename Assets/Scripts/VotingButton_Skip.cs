using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VotingButton_Skip : MonoBehaviour
{
    [Header("References")]
    public Button buttonSelf;
    public GameObject votePanel;
    public Button buttonYes;
    public Button buttonNo;

    [Header("Data")]
    public int skipIndex = 10; // Luôn là 10 cho nút Skip

    private bool hasVoted = false;

    void Start()
    {
        if (buttonSelf != null)
            buttonSelf.onClick.AddListener(OnButtonClicked);

        if (buttonYes != null)
            buttonYes.onClick.AddListener(OnClickYes);

        if (buttonNo != null)
            buttonNo.onClick.AddListener(OnClickNo);

        if (votePanel != null)
            votePanel.SetActive(false);
    }

    void OnButtonClicked()
    {
        if (hasVoted) return;

        VotingPanelManager.Instance?.CloseAllPanelsExcept(null);
        votePanel.SetActive(true);
    }

    void OnClickYes()
    {
        if (hasVoted) return;
        hasVoted = true;

        votePanel.SetActive(false);
        VotingDataManager.Instance.AddVote("Skip", 1);

        PlayerPrefs.SetInt("VotedOutIndex", skipIndex);
        Debug.Log($"[VOTE] Human voted SKIP (index {skipIndex})");

        VotingLockoutManager.LockAllButtonsExcept(this.gameObject);
    }

    void OnClickNo()
    {
        votePanel.SetActive(false);
    }
}
