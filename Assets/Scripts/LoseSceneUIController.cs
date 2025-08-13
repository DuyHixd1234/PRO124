using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoseSceneUIController : MonoBehaviour
{
    [Header("Canvas Lose (ngược lại Win)")]
    public GameObject crewmateCanvas;
    public GameObject impostorCanvas;

    [Header("Images Slots (9 slots hoặc 8 tuỳ)")]
    public Image[] crewmateImages;
    public Image[] impostorImages;

    [Header("Sprites Alive/Ghost (10 slots each)")]
    public Sprite[] aliveSprites = new Sprite[10];
    public Sprite[] ghostSprites = new Sprite[10];

    [Header("Ghost Transparency (0–1)")]
    [Range(0f, 1f)] public float ghostAlpha = 0.17f;

    [Header("TMP Text Impostor Names")]
    public TMP_Text impostorName1;
    public TMP_Text impostorName2;

    [Header("Impostor Sprites In Canvas")]
    public Image impostorImage1;
    public Image impostorImage2;

    void Start()
    {
        DumpAllPrefs();
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
        FillImages(crewmateImages, "Crewmate");
    }

    // ---------- IMPOSTOR CANVAS ----------
    void ShowImpostorCanvas()
    {
        SafeSetActive(crewmateCanvas, false);
        SafeSetActive(impostorCanvas, true);
        FillImages(impostorImages, "Impostor");
        FillImpostorNames();
    }

    void FillImpostorNames()
    {
        List<int> impostorIdx = new List<int>();
        List<string> impostorNames = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1) // 1 = impostor
            {
                impostorIdx.Add(NormalizeIndex(i));
                impostorNames.Add(PlayerPrefs.GetString($"Shuffle_Name_{i}", $"Imp_{i}"));
            }
        }

        if (impostorName1 != null)
            impostorName1.text = impostorNames.Count > 0 ? impostorNames[0] : "";

        if (impostorName2 != null)
            impostorName2.text = impostorNames.Count > 1 ? impostorNames[1] : "";

        if (impostorImage1 != null)
        {
            if (impostorIdx.Count > 0 && SpriteIndexValid(impostorIdx[0]))
            {
                impostorImage1.sprite = aliveSprites[impostorIdx[0]];
                SetImageAlpha(impostorImage1, 1f);
            }
            else
            {
                impostorImage1.sprite = null;
                SetImageAlpha(impostorImage1, 0f);
            }
        }

        if (impostorImage2 != null)
        {
            if (impostorIdx.Count > 1 && SpriteIndexValid(impostorIdx[1]))
            {
                impostorImage2.sprite = aliveSprites[impostorIdx[1]];
                SetImageAlpha(impostorImage2, 1f);
            }
            else
            {
                impostorImage2.sprite = null;
                SetImageAlpha(impostorImage2, 0f);
            }
        }

        Debug.Log($"[LoseUI] ImpostorIdx= {string.Join(",", impostorIdx)} | Names= {string.Join(",", impostorNames)}");
    }

    // ---------- FILL ----------
    void FillImages(Image[] imageSlots, string roleTag)
    {
        List<int> aliveIdx = new List<int>();
        List<int> deadIdx = new List<int>();

        string aliveCountKey = roleTag == "Crewmate" ? "AliveCrewmateCount" : "AliveImpostorCount";
        string deadCountKey = roleTag == "Crewmate" ? "DeadCrewmateCount" : "DeadImpostorCount";
        string aliveIndexPrefix = roleTag == "Crewmate" ? "AliveCrewmateIndex_" : "AliveImpostorIndex_";
        string deadIndexPrefix = roleTag == "Crewmate" ? "DeadCrewmateIndex_" : "DeadImpostorIndex_";

        aliveIdx.AddRange(GetIndexArrayByCountKey(aliveCountKey, aliveIndexPrefix));
        deadIdx.AddRange(GetIndexArrayByCountKey(deadCountKey, deadIndexPrefix));

        if (aliveIdx.Count == 0 && deadIdx.Count == 0)
        {
            string aliveCsvKey = roleTag == "Crewmate" ? "Alive_Crew" : "Alive_Imp";
            string deadCsvKey = roleTag == "Crewmate" ? "Dead_Crew" : "Dead_Imp";
            aliveIdx.AddRange(GetIndexArrayFromCsv(aliveCsvKey));
            deadIdx.AddRange(GetIndexArrayFromCsv(deadCsvKey));
        }

        if (aliveIdx.Count == 0 && deadIdx.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                string tag = PlayerPrefs.GetString($"Tag_Index_{i}", "");
                if (tag == roleTag)
                {
                    if (PlayerPrefs.GetInt($"Alive_Index_{i}", 0) == 1) aliveIdx.Add(NormalizeIndex(i));
                    else if (PlayerPrefs.GetInt($"Dead_Index_{i}", 0) == 1) deadIdx.Add(NormalizeIndex(i));
                }
            }
        }

        Debug.Log($"[LoseUI] {roleTag} Alive={string.Join(",", aliveIdx)} | Dead={string.Join(",", deadIdx)}");

        for (int i = 0; i < imageSlots.Length; i++)
        {
            if (imageSlots[i] != null)
            {
                imageSlots[i].sprite = null;
                SetImageAlpha(imageSlots[i], 0f);
            }
        }

        int filled = 0;
        foreach (int idx in aliveIdx)
        {
            if (filled >= imageSlots.Length) break;
            if (!SpriteIndexValid(idx)) continue;
            imageSlots[filled].sprite = aliveSprites[idx];
            SetImageAlpha(imageSlots[filled], 1f);
            filled++;
        }
        foreach (int idx in deadIdx)
        {
            if (filled >= imageSlots.Length) break;
            if (!SpriteIndexValid(idx)) continue;
            imageSlots[filled].sprite = ghostSprites[idx];
            SetImageAlpha(imageSlots[filled], ghostAlpha);
            filled++;
        }

        Debug.Log($"[LoseUI] Filled {filled}/{imageSlots.Length} images for {roleTag}.");
    }

    // ---------- HELPERS ----------
    int[] GetIndexArrayByCountKey(string countKey, string indexKeyPrefix)
    {
        int count = PlayerPrefs.GetInt(countKey, 0);
        List<int> result = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            int val = PlayerPrefs.GetInt(indexKeyPrefix + i, -1);
            if (val > 0) val = NormalizeIndex(val);
            if (val >= 0) result.Add(val);
        }
        return result.ToArray();
    }

    int[] GetIndexArrayFromCsv(string key)
    {
        string raw = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(raw)) return new int[0];
        string[] parts = raw.Split(',');
        List<int> res = new List<int>(parts.Length);
        foreach (var p in parts)
        {
            if (int.TryParse(p, out int v))
            {
                if (v > 0) v = NormalizeIndex(v);
                if (v >= 0) res.Add(v);
            }
        }
        return res.ToArray();
    }

    int NormalizeIndex(int idx)
    {
        // Nếu dữ liệu lưu theo kiểu 1-based thì trừ 1
        // Ví dụ: 1 -> 0, 2 -> 1, ..., 10 -> 9
        return Mathf.Clamp(idx - 1, 0, 9);
    }

    bool SpriteIndexValid(int idx)
    {
        bool ok = (idx >= 0 && idx < aliveSprites.Length && idx < ghostSprites.Length
                   && aliveSprites[idx] != null && ghostSprites[idx] != null);
        if (!ok) Debug.LogWarning($"[LoseUI] Sprite index {idx} invalid or null.");
        return ok;
    }

    void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        var c = img.color; c.a = alpha; img.color = c;
    }

    void SafeSetActive(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }

    void DumpAllPrefs()
    {
        Debug.Log("------ [LoseUI] Dump PlayerPrefs Data ------");
        string[] keys = {
            "AliveImpostorCount","DeadImpostorCount",
            "AliveCrewmateCount","DeadCrewmateCount",
            "Alive_Crew","Dead_Crew","Dead_Imp","Alive_Imp"
        };
        foreach (string k in keys)
        {
            if (PlayerPrefs.HasKey(k))
                Debug.Log($"{k} = {PlayerPrefs.GetString(k, PlayerPrefs.GetInt(k, 0).ToString())}");
            else
                Debug.Log($"{k} = <no key>");
        }

        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"AliveImp_{i} = {PlayerPrefs.GetInt($"AliveImpostorIndex_{i}", -1)}");
            Debug.Log($"DeadImp_{i} = {PlayerPrefs.GetInt($"DeadImpostorIndex_{i}", -1)}");
            Debug.Log($"AliveCrew_{i} = {PlayerPrefs.GetInt($"AliveCrewmateIndex_{i}", -1)}");
            Debug.Log($"DeadCrew_{i} = {PlayerPrefs.GetInt($"DeadCrewmateIndex_{i}", -1)}");
            Debug.Log($"Alive_Index_{i} = {PlayerPrefs.GetInt($"Alive_Index_{i}", -1)} | Dead_Index_{i} = {PlayerPrefs.GetInt($"Dead_Index_{i}", -1)} | Tag = {PlayerPrefs.GetString($"Tag_Index_{i}", "")}");
        }
        Debug.Log("------------------------------------------");
    }
}
