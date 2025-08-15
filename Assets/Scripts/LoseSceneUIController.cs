using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoseSceneUIController : MonoBehaviour
{
    [Header("Canvas Lose (ngược lại Win)")]
    public GameObject crewmateCanvas;
    public GameObject impostorCanvas;

    [Header("Images Slots (8 slots)")]
    public Image[] crewmateImages; // cho crewmate
    public Image[] impostorImages; // cho impostor

    [Header("TMP Text Impostor Names")]
    public TMP_Text impostorName1;
    public TMP_Text impostorName2;

    [Header("Impostor Sprites In Canvas")]
    public Image impostorImage1;
    public Image impostorImage2;

    [Header("Sprites (match Shuffle.cs names)")]
    public Sprite[] aiSprites = new Sprite[9]; // index 1–9 trong shuffle

    void Start()
    {
        LoadLoseData();
    }

    void LoadLoseData()
    {
        bool playerIsImpostor = PlayerPrefs.GetInt("Shuffle_Role_0", 0) == 1;
        Debug.Log($"[LoseUI] PlayerIsImpostor={playerIsImpostor}");

        if (playerIsImpostor) ShowCrewmateCanvas();
        else ShowImpostorCanvas();
    }

    // ---------- CREWMATE CANVAS ----------
    void ShowCrewmateCanvas()
    {
        SafeSetActive(crewmateCanvas, true);
        SafeSetActive(impostorCanvas, false);
        FillCrewmateImages(crewmateImages);
    }

    // ---------- IMPOSTOR CANVAS ----------
    void ShowImpostorCanvas()
    {
        SafeSetActive(crewmateCanvas, false);
        SafeSetActive(impostorCanvas, true);
        FillImpostorImages(impostorImages);
        FillImpostorNames();
    }

    // ---------- FILL CREWMATE ----------
    void FillCrewmateImages(Image[] slots)
    {
        List<Sprite> crewSprites = new List<Sprite>();

        for (int i = 1; i < 10; i++) // chỉ AI
        {
            bool isImp = PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1;
            if (isImp) continue; // bỏ impostor

            string spriteName = PlayerPrefs.GetString($"Shuffle_Sprite_{i}", "");
            Sprite found = FindSpriteByName(spriteName);
            if (found != null) crewSprites.Add(found);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < crewSprites.Count) slots[i].sprite = crewSprites[i];
            else slots[i].sprite = null;
        }

        Debug.Log($"[LoseUI] Filled {crewSprites.Count} crew sprites into {slots.Length} slots.");
    }

    // ---------- FILL IMPOSTOR ----------
    void FillImpostorImages(Image[] slots)
    {
        List<Sprite> impSprites = new List<Sprite>();

        for (int i = 1; i < 10; i++) // chỉ AI
        {
            bool isImp = PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1;
            if (!isImp) continue; // bỏ crewmate

            string spriteName = PlayerPrefs.GetString($"Shuffle_Sprite_{i}", "");
            Sprite found = FindSpriteByName(spriteName);
            if (found != null) impSprites.Add(found);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < impSprites.Count) slots[i].sprite = impSprites[i];
            else slots[i].sprite = null;
        }

        Debug.Log($"[LoseUI] Filled {impSprites.Count} impostor sprites into {slots.Length} slots.");
    }

    void FillImpostorNames()
    {
        List<string> impNames = new List<string>();
        List<Sprite> impSpriteList = new List<Sprite>();

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1)
            {
                impNames.Add(PlayerPrefs.GetString($"Shuffle_Name_{i}", $"Imp_{i}"));
                Sprite found = FindSpriteByName(PlayerPrefs.GetString($"Shuffle_Sprite_{i}", ""));
                impSpriteList.Add(found);
            }
        }

        impostorName1.text = impNames.Count > 0 ? impNames[0] : "";
        impostorName2.text = impNames.Count > 1 ? impNames[1] : "";

        impostorImage1.sprite = impSpriteList.Count > 0 ? impSpriteList[0] : null;
        impostorImage2.sprite = impSpriteList.Count > 1 ? impSpriteList[1] : null;
    }

    // ---------- HELPERS ----------
    Sprite FindSpriteByName(string spriteName)
    {
        foreach (var s in aiSprites)
        {
            if (s != null && s.name == spriteName) return s;
        }
        Debug.LogWarning($"[LoseUI] Sprite '{spriteName}' not found in aiSprites array.");
        return null;
    }

    void SafeSetActive(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }
}
