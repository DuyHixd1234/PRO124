using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HSLoseResultUI : MonoBehaviour
{
    [Header("UI Canvas")]
    public GameObject canvasCrewmate;
    public GameObject canvasImpostor;

    [Header("UI Elements for AI Impostor Win")]
    public TMP_Text impostorNameText;
    public Image impostorImage;

    [Header("AI Alive Sprites (Chỉ cho AI, không bao gồm Player)")]
    public Sprite[] aiAliveSprites; // 9 sprite AI còn sống (tương ứng entityList[1]..entityList[9] của scene trước)

    void Start()
    {
        if (canvasCrewmate != null) canvasCrewmate.SetActive(false);
        if (canvasImpostor != null) canvasImpostor.SetActive(false);

        // Lấy dữ liệu từ PlayerPrefs (đồng bộ key với DeathListManager)
        bool playerWasImpostor = PlayerPrefs.GetInt("Player_IsImpostor", 0) == 1;
        int impostorIndex = PlayerPrefs.GetInt("ImpostorIndex", -1); // 0–9 (scene trước)

        if (playerWasImpostor)
        {
            // Nếu Player là impostor và thua → Crewmate thắng
            if (canvasCrewmate != null) canvasCrewmate.SetActive(true);
        }
        else
        {
            // Impostor win → bật canvas impostor
            if (canvasImpostor != null) canvasImpostor.SetActive(true);

            string impostorName = PlayerPrefs.GetString($"Shuffle_Name_{impostorIndex}", "Unknown");
            if (impostorNameText != null)
                impostorNameText.text = impostorName;

            // Map index: entityList[1] → aiAliveSprites[0]
            if (impostorIndex > 0)
            {
                int spriteIndex = impostorIndex - 1;
                if (spriteIndex >= 0 && spriteIndex < aiAliveSprites.Length)
                    impostorImage.sprite = aiAliveSprites[spriteIndex];
                else
                    Debug.LogWarning($"Không tìm thấy sprite cho AI Impostor index {impostorIndex} (spriteIndex={spriteIndex})");
            }
            else
            {
                Debug.LogWarning("ImpostorIndex = 0 → là Player, sprite không nằm trong AI Alive Sprites!");
            }
        }
    }
}
