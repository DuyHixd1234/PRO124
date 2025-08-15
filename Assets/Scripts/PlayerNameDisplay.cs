using UnityEngine;
using TMPro;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    private void Awake()
    {
        if (nameText == null)
        {
            nameText = GetComponent<TMP_Text>();
        }
    }

    private void Start()
    {
        string playerName = PlayerPrefs.GetString("Shuffle_Name_0", "Player");

        // Câu text với tên màu vàng
        string finalText = $"CREWMEMBER <color=#FF0000>{playerName}</color> HAS";

        if (nameText != null)
        {
            nameText.text = finalText;
        }
        else
        {
           // Debug.LogError("[PlayerNameDisplay] TMP_Text chưa được gán!");
        }
    }
}
