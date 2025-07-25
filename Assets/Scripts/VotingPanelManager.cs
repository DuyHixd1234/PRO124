using System.Collections.Generic;
using UnityEngine;

public class VotingPanelManager : MonoBehaviour
{
    public static VotingPanelManager Instance;

    private List<VotingButton> buttons = new List<VotingButton>();

    [Header("UI Effect")]
    public GameObject readyVoteObject; // GameObject sẽ bật khi có ít nhất 1 votePanel mở

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(VotingButton btn)
    {
        if (!buttons.Contains(btn))
            buttons.Add(btn);
    }

    public void CloseAllPanelsExcept(VotingButton exclude)
    {
        foreach (var btn in buttons)
        {
            if (btn != exclude)
                btn.ClosePanel();
        }
    }

    void Update()
    {
        // Kiem tra neu co it nhat 1 votePanel dang mo => bat readyVoteObject
        bool anyPanelOpen = false;

        foreach (var btn in buttons)
        {
            if (btn.votePanel != null && btn.votePanel.activeSelf)
            {
                anyPanelOpen = true;
                break;
            }
        }

        if (readyVoteObject != null)
            readyVoteObject.SetActive(anyPanelOpen);
    }
}
