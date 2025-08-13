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

    [Header("Sprites Alive/Ghost (10 slots each)")]
    public Sprite[] aliveSprites = new Sprite[10];
    public Sprite[] ghostSprites = new Sprite[10];

    [Header("Ghost Transparency (0–1)")]
    [Range(0f, 1f)] public float ghostAlpha = 0.17f;

    void Start()
    {
        DumpAllPrefs();
        LoadWinData();
    }

    void LoadWinData()
    {
        Debug.Log("[WinUI] === Dump all PlayerPrefs keys ===");
        Debug.Log($"AliveImpostorCount={PlayerPrefs.GetInt("AliveImpostorCount", 0)}");
        Debug.Log($"AliveCrewmateCount={PlayerPrefs.GetInt("AliveCrewmateCount", 0)}");
        Debug.Log($"DeadImpostorCount={PlayerPrefs.GetInt("DeadImpostorCount", 0)}");
        Debug.Log($"DeadCrewmateCount={PlayerPrefs.GetInt("DeadCrewmateCount", 0)}");

        bool playerIsImpostor = PlayerPrefs.GetInt("Shuffle_Role_0", 0) == 1;
        Debug.Log($"[WinUI] PlayerIsImpostor={playerIsImpostor}");

        if (playerIsImpostor) ShowImpostorCanvas();
        else ShowCrewmateCanvas();
    }

    // ---------- CREWMATE CANVAS ----------
    void ShowCrewmateCanvas()
    {
        SafeSetActive(crewmateCanvas, true);
        SafeSetActive(impostorCanvas, false);

        int[] crewAlive = GetIndexArrayByCountKey("AliveCrewmateCount", "AliveCrewmateIndex_");
        int[] crewDead = GetIndexArrayByCountKey("DeadCrewmateCount", "DeadCrewmateIndex_");

        // Fallback CSV
        if (crewAlive.Length == 0 && crewDead.Length == 0)
        {
            crewAlive = GetIndexArrayFromCsv("Alive_Crew");
            crewDead = GetIndexArrayFromCsv("Dead_Crew");
            if (crewAlive.Length > 0 || crewDead.Length > 0)
                Debug.Log("[WinUI] Fallback CSV data.");
        }

        // Fallback Alive/Dead_Index
        if (crewAlive.Length == 0 && crewDead.Length == 0)
        {
            List<int> aliveList = new List<int>();
            List<int> deadList = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                string tag = PlayerPrefs.GetString($"Tag_Index_{i}", "");
                if (tag == "Crewmate")
                {
                    int aliveVal = PlayerPrefs.GetInt($"Alive_Index_{i}", 0);
                    int deadVal = PlayerPrefs.GetInt($"Dead_Index_{i}", 0);

                    if (aliveVal == 1) aliveList.Add(i);
                    else if (deadVal == 1) deadList.Add(i);
                }
            }
            crewAlive = aliveList.ToArray();
            crewDead = deadList.ToArray();
            Debug.Log("[WinUI] Fallback Alive/Dead_Index data.");
        }

        Debug.Log($"[WinUI] Crew Alive={string.Join(",", crewAlive)} | Dead={string.Join(",", crewDead)}");

        // Chỉ reset nếu có dữ liệu mới
        if (crewAlive.Length > 0 || crewDead.Length > 0)
        {
            // Reset images
            for (int i = 0; i < crewmateImages.Length; i++)
            {
                if (crewmateImages[i] != null)
                {
                    crewmateImages[i].sprite = null;
                    SetImageAlpha(crewmateImages[i], 0f);
                }
            }

            int filled = 0;

            foreach (int idx in crewAlive)
            {
                if (filled >= crewmateImages.Length) break;
                if (!SpriteIndexValid(idx)) continue;

                crewmateImages[filled].sprite = aliveSprites[idx];
                SetImageAlpha(crewmateImages[filled], 1f);
                filled++;
            }

            foreach (int idx in crewDead)
            {
                if (filled >= crewmateImages.Length) break;
                if (!SpriteIndexValid(idx)) continue;

                crewmateImages[filled].sprite = ghostSprites[idx];
                SetImageAlpha(crewmateImages[filled], ghostAlpha);
                filled++;
            }

            Debug.Log($"[WinUI] Filled {filled}/{crewmateImages.Length} crew images.");
        }
        else
        {
            Debug.Log("[WinUI] No crew data found, keeping existing images.");
        }
    }

    // ---------- IMPOSTOR CANVAS ----------
    void ShowImpostorCanvas()
    {
        SafeSetActive(crewmateCanvas, false);
        SafeSetActive(impostorCanvas, true);

        string playerName = PlayerPrefs.GetString("Shuffle_Name_0", "Player");
        bool playerDead = IsIndexDead(0);
        SetPortrait(playerImage, 0, playerDead);
        if (playerNameText) playerNameText.text = playerName;

        bool foundAI = false;
        for (int i = 1; i < 10; i++)
        {
            if (PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0) == 1)
            {
                string aiName = PlayerPrefs.GetString($"Shuffle_Name_{i}", $"AI{i}");
                bool aiDead = IsIndexDead(i);
                SetPortrait(aiImage, i, aiDead);
                if (aiNameText) aiNameText.text = aiName;
                foundAI = true;
                break;
            }
        }

        if (!foundAI)
        {
            if (aiImage) { aiImage.sprite = null; SetImageAlpha(aiImage, 0f); }
            if (aiNameText) aiNameText.text = "";
            Debug.Log("[WinUI] No AI impostor found.");
        }
    }

    // ---------- HELPERS ----------
    void SetPortrait(Image img, int idx, bool isGhost)
    {
        if (img == null) return;
        if (!SpriteIndexValid(idx)) { SetImageAlpha(img, 0f); return; }

        img.sprite = isGhost ? ghostSprites[idx] : aliveSprites[idx];
        SetImageAlpha(img, isGhost ? ghostAlpha : 1f);
    }

    bool IsIndexDead(int idx)
    {
        var deadCrew = new HashSet<int>(GetIndexArrayByCountKey("DeadCrewmateCount", "DeadCrewmateIndex_"));
        var deadImp = new HashSet<int>(GetIndexArrayByCountKey("DeadImpostorCount", "DeadImpostorIndex_"));

        if (deadCrew.Count == 0 && deadImp.Count == 0)
        {
            foreach (var d in GetIndexArrayFromCsv("Dead_Crew")) deadCrew.Add(d);
            foreach (var d in GetIndexArrayFromCsv("Dead_Imp")) deadImp.Add(d);

            // Fallback Alive/Dead_Index
            if (deadCrew.Count == 0 && deadImp.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int deadVal = PlayerPrefs.GetInt($"Dead_Index_{i}", 0);
                    string tag = PlayerPrefs.GetString($"Tag_Index_{i}", "");
                    if (deadVal == 1)
                    {
                        if (tag == "Crewmate") deadCrew.Add(i);
                        if (tag == "Impostor") deadImp.Add(i);
                    }
                }
            }
        }

        return deadCrew.Contains(idx) || deadImp.Contains(idx);
    }

    int[] GetIndexArrayByCountKey(string countKey, string indexKeyPrefix)
    {
        int count = PlayerPrefs.GetInt(countKey, 0);
        List<int> result = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            int val = PlayerPrefs.GetInt(indexKeyPrefix + i, -1);
            Debug.Log($"[WinUI] Read {indexKeyPrefix}{i} = {val}");
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
            if (int.TryParse(p, out int v)) res.Add(v);
        }
        return res.ToArray();
    }

    void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        var c = img.color; c.a = alpha; img.color = c;
    }

    bool SpriteIndexValid(int idx)
    {
        bool ok = (idx >= 0 && idx < aliveSprites.Length && idx < ghostSprites.Length
                   && aliveSprites[idx] != null && ghostSprites[idx] != null);
        if (!ok) Debug.LogWarning($"[WinUI] Sprite index {idx} invalid or null.");
        return ok;
    }

    void SafeSetActive(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }

    void DumpAllPrefs()
    {
        Debug.Log("------ [WinUI] Dump PlayerPrefs Data ------");
        string[] keys = {
            "AliveImpostorCount","DeadImpostorCount",
            "AliveCrewmateCount","DeadCrewmateCount",
            "Alive_Crew","Dead_Crew","Dead_Imp"
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
