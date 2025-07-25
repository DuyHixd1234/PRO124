using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class VotingButton : MonoBehaviour
{
    [Header("References")]
    public GameObject statusCheckObject;
    public Image imageXRed;
    public Image imageIVoted;
    public Button buttonSelf;
    public TMP_Text nameText;
    public GameObject votePanel;
    public Button buttonYes;
    public Button buttonNo;
    public Button buttonItsHim;

    [Header("Data")]
    public int characterIndex;
    public bool isPlayer;
    public GameObject voteAllocatorTrigger;

    public TMP_Text voteCountTMP;

    private string characterName;
    private bool hasVoted = false;
    private bool isDead = false;

    void Start()
    {
        if (isPlayer)
        {
            VotingUIHelper.Instance.playerButton = this;
        }

        characterName = PlayerPrefs.GetString($"Shuffle_Name_{characterIndex}", $"Unknown {characterIndex}");
        nameText.text = characterName;

        VotingPanelManager.Instance?.Register(this);

        if (statusCheckObject != null && !statusCheckObject.activeSelf)
        {
            SetDeadState();
        }
        else
        {
            SetAliveState();
        }

        buttonSelf.onClick.AddListener(OnButtonClicked);
        buttonNo.onClick.AddListener(OnClickNo);

        votePanel.SetActive(false);
        imageIVoted.gameObject.SetActive(false);

        if (voteCountTMP != null)
        {
            voteCountTMP.text = "";
            voteCountTMP.gameObject.SetActive(false);
        }
    }

    void SetDeadState()
    {
        isDead = true;
        buttonSelf.interactable = false;

        var btnColor = buttonSelf.colors;
        btnColor.normalColor = new Color(1f, 1f, 1f, 0.5f);
        buttonSelf.colors = btnColor;

        imageXRed.gameObject.SetActive(true);
        imageIVoted.gameObject.SetActive(false);
        votePanel.SetActive(false);
    }

    void SetAliveState()
    {
        isDead = false;
        buttonSelf.interactable = true;

        var btnColor = buttonSelf.colors;
        btnColor.normalColor = new Color(1f, 1f, 1f, 1f);
        buttonSelf.colors = btnColor;

        imageXRed.gameObject.SetActive(false);
    }

    void OnButtonClicked()
    {
        if (hasVoted || isDead) return;

        VotingPanelManager.Instance?.CloseAllPanelsExcept(this);
        votePanel.SetActive(true);

        string role = PlayerPrefs.GetString("PlayerRole", "Crewmate");
        buttonItsHim.gameObject.SetActive(role == "Crewmate");

        //Debug.Log("ROLE: " + role);
    }

    void OnClickYes()
    {
        if (hasVoted) return;

        hasVoted = true;
        votePanel.SetActive(false);

        VotingDataManager.Instance.AddVote(characterName, 1);
        VotingLockoutManager.LockAllButtonsExcept(this.gameObject);
        VotingUIHelper.Instance.ShowHumanIVoted();

        PlayerPrefs.SetInt("VotedOutIndex", characterIndex);
        Debug.Log($"[VOTE] {characterName} bị vote (index {characterIndex})");

        if (isPlayer && voteAllocatorTrigger != null)
            voteAllocatorTrigger.SetActive(true);
    }

    void OnClickNo()
    {
        votePanel.SetActive(false);
    }

    void OnClickItsHim()
    {
        if (hasVoted) return;

        hasVoted = true;
        votePanel.SetActive(false);

        VotingDataManager.Instance.AddVote(characterName, 10);
        VotingDataManager.Instance.MarkForceVote();
        VotingDataManager.Instance.SetForceVoteTarget(characterName);

        PlayerPrefs.SetInt("VotedOutIndex", characterIndex);
        Debug.Log($"[FORCE VOTE] {characterName} bị ép vote (index {characterIndex})");

        VotingLockoutManager.LockAllButtonsExcept(this.gameObject);
        VotingUIHelper.Instance?.ShowHumanIVoted();

        if (isPlayer && voteAllocatorTrigger != null)
            voteAllocatorTrigger.SetActive(true);
    }

    public void ClosePanel()
    {
        votePanel.SetActive(false);
    }

    // ✅ Gọi hàm này khi cần hiển thị số vote
    public void ShowVoteCount()
    {
        if (voteCountTMP == null) return;

        int targetVotes = VotingDataManager.Instance.voteCounts.ContainsKey(characterName) ? VotingDataManager.Instance.voteCounts[characterName] : 0;

        voteCountTMP.gameObject.SetActive(true);
        StartCoroutine(AnimateVoteCount(targetVotes));
    }

    IEnumerator AnimateVoteCount(int target)
    {
        int current = 0;
        float delay = 0.2f;

        while (current <= target)
        {
            voteCountTMP.text = current.ToString();

            // Giật nhẹ text mỗi lần tăng
            voteCountTMP.transform.localScale = Vector3.one * 1.2f;
            yield return new WaitForSeconds(0.05f);
            voteCountTMP.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(delay);
            current++;
        }
    }
}
