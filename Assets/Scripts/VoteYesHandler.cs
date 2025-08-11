using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteYesHandler : MonoBehaviour
{
    public Button yesButton;                     // Button YES trong panel
    public TMP_Text nameText;                    // TMP chứa tên nhân vật này (chỉ để hiển thị)
    public GameObject voteAllocatorTrigger;      // Kích hoạt tiến trình sau cùng

    public int characterIndex;                   // Human = 0, AI = 1–9

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

        // Lấy tên đúng từ Shuffle_Name_{index}
        string characterName = PlayerPrefs.GetString($"Shuffle_Name_{characterIndex}", $"Unknown_{characterIndex}");

        // Gửi vote đúng tên
        VotingDataManager.Instance.AddVote(characterName, 1);

        // Ghi nhận người bị vote
        PlayerPrefs.SetInt("VotedOutIndex", characterIndex);

        // Debug thông tin vote
        Debug.Log($"✅ Trigger bật cho {characterName} (index {characterIndex})");

        // Bật tiến trình tiếp theo nếu có
        if (voteAllocatorTrigger != null)
            voteAllocatorTrigger.SetActive(true);
    }
}
