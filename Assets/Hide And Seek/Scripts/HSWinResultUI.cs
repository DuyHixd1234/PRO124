using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HSWinResultUI : MonoBehaviour
{
    [Header("UI Canvas")]
    public GameObject canvasCrewmate;
    public GameObject canvasImpostor;

    [Header("UI Elements for Impostor")]
    public TMP_Text impostorNameText;
    public Image impostorImage;
    public Sprite[] impostorColorSprites; // 4 màu player: red, yellow, green, white

    void Start()
    {
        // Mặc định tắt cả hai canvas
        if (canvasCrewmate != null) canvasCrewmate.SetActive(false);
        if (canvasImpostor != null) canvasImpostor.SetActive(false);

        // Lấy dữ liệu role của người chơi (index 0)
        int playerRole = PlayerPrefs.GetInt("Shuffle_Role_0", 0); // 1 = Impostor, 0 = Crewmate
        string playerName = PlayerPrefs.GetString("Shuffle_Name_0", "Unknown");

        // Lấy màu player từ PlayerData hoặc PlayerPrefs
        int playerColorIndex = PlayerPrefs.GetInt("PlayerColorIndex", -1);

        if (playerRole == 1) // Người chơi là Impostor
        {
            if (canvasImpostor != null) canvasImpostor.SetActive(true);

            if (impostorNameText != null)
                impostorNameText.text = playerName;

            if (impostorImage != null && playerColorIndex >= 0 && playerColorIndex < impostorColorSprites.Length)
                impostorImage.sprite = impostorColorSprites[playerColorIndex];
        }
        else // Người chơi là Crewmate
        {
            if (canvasCrewmate != null) canvasCrewmate.SetActive(true);
        }
    }
}
