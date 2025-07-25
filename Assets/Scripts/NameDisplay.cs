using UnityEngine;
using TMPro;

public class NameDisplay : MonoBehaviour
{
    public TMP_Text nameText;

    void Start()
    {
        if (PlayerData.Instance != null && nameText != null)
        {
            nameText.text = PlayerData.Instance.playerName;
        }
    }
}
