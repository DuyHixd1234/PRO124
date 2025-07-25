using UnityEngine;
using UnityEngine.UI;

public class VotingUIHelper : MonoBehaviour
{
    public static VotingUIHelper Instance;
    [HideInInspector] public VotingButton playerButton;

    public Image humanIVotedImage;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowHumanIVoted()
    {
        if (playerButton != null && playerButton.imageIVoted != null)
        {
            playerButton.imageIVoted.gameObject.SetActive(true);
        }
    }
}
