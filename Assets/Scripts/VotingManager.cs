using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VotingManager : MonoBehaviour
{
    [System.Serializable]
    public class VoterSlot
    {
        public Button voteButton;
        public TextMeshProUGUI nameText;
        public Image redX;
        public Image speakerIcon;
        public GameObject confirmVotePanel;
        public Button confirmButton;
        public Button cancelButton;
        public Image iVotedImage;

        [HideInInspector] public string playerName;
        [HideInInspector] public bool isDead = false;
        [HideInInspector] public bool hasVoted = false;
    }

    public List<VoterSlot> voterSlots;
    public Button skipButton;

    private int totalVotes = 0;
    private string reporterName = "";

    void Start()
    {
        LoadPlayerData();
        SetupVotingButtons();
        skipButton.onClick.AddListener(() => {
            RegisterVote("SKIP");
        });
    }

    void LoadPlayerData()
    {
        for (int i = 0; i < voterSlots.Count; i++)
        {
            var slot = voterSlots[i];
            // Gán tên từ dữ liệu lưu trước
            slot.playerName = PlayerPrefs.GetString("Name_" + i, "Name");
            slot.nameText.text = slot.playerName;

            // Kiểm tra nếu chết
            bool isDead = PlayerPrefs.GetInt("Dead_" + i, 0) == 1;
            slot.isDead = isDead;
            slot.redX.gameObject.SetActive(isDead);

            // Nếu chết thì không cho vote
            slot.voteButton.interactable = !isDead;
            var colors = slot.voteButton.colors;
            colors.normalColor = isDead ? new Color(1f, 1f, 1f, 0.4f) : Color.white;
            slot.voteButton.colors = colors;

            // Ẩn icon loa và I voted
            slot.speakerIcon.gameObject.SetActive(false);
            slot.iVotedImage.gameObject.SetActive(false);

            // Tắt panel xác nhận
            slot.confirmVotePanel.SetActive(false);
        }

        // Hiển thị ai là người report
        reporterName = PlayerPrefs.GetString("ReporterName", "");
        foreach (var slot in voterSlots)
        {
            if (slot.playerName == reporterName)
                slot.speakerIcon.gameObject.SetActive(true);
        }
    }

    void SetupVotingButtons()
    {
        for (int i = 0; i < voterSlots.Count; i++)
        {
            int index = i; // fix closure
            voterSlots[i].voteButton.onClick.AddListener(() =>
            {
                if (voterSlots[index].isDead || voterSlots[index].hasVoted) return;

                ShowConfirmPanel(index);
            });

            voterSlots[i].confirmButton.onClick.AddListener(() =>
            {
                RegisterVote(voterSlots[index].playerName);
                voterSlots[index].confirmVotePanel.SetActive(false);
            });

            voterSlots[i].cancelButton.onClick.AddListener(() =>
            {
                voterSlots[index].confirmVotePanel.SetActive(false);
            });
        }
    }

    void ShowConfirmPanel(int index)
    {
        Vector3 spawnPos = Input.mousePosition;
        voterSlots[index].confirmVotePanel.SetActive(true);
        voterSlots[index].confirmVotePanel.transform.position = spawnPos;
    }

    void RegisterVote(string targetName)
    {
        string voter = PlayerPrefs.GetString("PlayerName"); // Người chơi
        Debug.Log(voter + " voted for " + targetName);

        totalVotes++;

        // Hiển thị dấu "I Voted"
        foreach (var slot in voterSlots)
        {
            if (slot.playerName == voter)
            {
                slot.iVotedImage.gameObject.SetActive(true);
                slot.hasVoted = true;
                break;
            }
        }

        // TODO: Ghi dữ liệu vote vào hệ thống nếu cần (ví dụ Dictionary)

        // Nếu tất cả đã vote (trừ người chết), thì xử lý kết quả
        if (totalVotes >= GetAliveCount())
        {
            Invoke("GoToVoteResultScene", 1f); // delay 1s
        }
    }

    int GetAliveCount()
    {
        int alive = 0;
        foreach (var slot in voterSlots)
        {
            if (!slot.isDead)
                alive++;
        }
        return alive;
    }

    void GoToVoteResultScene()
    {
        SceneManager.LoadScene("VoteResult"); // hoặc Map tiếp
    }
}
