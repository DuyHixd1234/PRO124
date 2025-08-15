using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WinSceneUIController : MonoBehaviour
{
    [Header("Canvas Win/Lose")]
    public GameObject crewmateCanvas;
    public GameObject impostorCanvas;

    [Header("Crewmate Images (8 slots)")]
    public Image[] crewmateImages = new Image[8];

    [Header("Impostor Images")]
    public Image playerImage;
    public Image aiImage;

    [Header("Impostor Names")]
    public TMP_Text playerNameText;
    public TMP_Text aiNameText;

    [Header("All Possible Sprites (match Shuffle.cs names)")]
    public Sprite redPlayerSprite;
    public Sprite[] aiSprites = new Sprite[9]; // Index 0–8 tương ứng Shuffle index 1–9

    void Start()
    {
        LoadWinData();
    }

    void LoadWinData()
    {
        bool playerIsImpostor = PlayerPrefs.GetInt("Shuffle_Role_0", 0) == 1;

        if (playerIsImpostor) ShowImpostorCanvas();
        else ShowCrewmateCanvas();
    }

    // ---------- CREWMATE CANVAS ----------
    void ShowCrewmateCanvas()
    {
        SafeSetActive(crewmateCanvas, true);
        SafeSetActive(impostorCanvas, false);

        // Clear tất cả slot trước
        for (int i = 0; i < crewmateImages.Length; i++)
        {
            if (crewmateImages[i] != null)
            {
                crewmateImages[i].sprite = null;
            }
        }

        // Điền Player (red) vào slot 0
        crewmateImages[0].sprite = redPlayerSprite;

        int filled = 1; // đã điền Player

        // Lấy các AI crewmate từ Shuffle
        for (int i = 1; i < 10; i++)
        {
            bool isImp = PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1;
            if (isImp) continue; // bỏ qua impostor

            string spriteName = PlayerPrefs.GetString($"Shuffle_Sprite_{i}", "");
            Sprite foundSprite = FindSpriteByName(spriteName);
            if (foundSprite != null && filled < crewmateImages.Length)
            {
                crewmateImages[filled].sprite = foundSprite;
                filled++;
            }
        }

        Debug.Log($"[WinUI] Filled {filled}/8 crew images (including player).");
    }

    // ---------- IMPOSTOR CANVAS ----------
    void ShowImpostorCanvas()
    {
        SafeSetActive(crewmateCanvas, false);
        SafeSetActive(impostorCanvas, true);

        // Player impostor
        playerImage.sprite = redPlayerSprite;
        if (playerNameText) playerNameText.text = PlayerPrefs.GetString("Shuffle_Name_0", "Player");

        // Tìm 1 AI impostor
        for (int i = 1; i < 10; i++)
        {
            if (PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1)
            {
                string spriteName = PlayerPrefs.GetString($"Shuffle_Sprite_{i}", "");
                Sprite foundSprite = FindSpriteByName(spriteName);
                if (foundSprite != null) aiImage.sprite = foundSprite;

                if (aiNameText) aiNameText.text = PlayerPrefs.GetString($"Shuffle_Name_{i}", $"AI{i}");
                break;
            }
        }
    }

    // ---------- HELPERS ----------
    Sprite FindSpriteByName(string spriteName)
    {
        if (spriteName == redPlayerSprite.name) return redPlayerSprite;

        foreach (var s in aiSprites)
        {
            if (s != null && s.name == spriteName) return s;
        }
        Debug.LogWarning($"[WinUI] Sprite '{spriteName}' not found in assigned arrays.");
        return null;
    }

    void SafeSetActive(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }
}
