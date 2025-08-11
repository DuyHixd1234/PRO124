using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeathDisplayManager : MonoBehaviour
{
    [Header("Gán 9 AI theo màu")]
    public GameObject Blue;
    public GameObject Cyan;
    public GameObject Coral;
    public GameObject Brown;
    public GameObject Purple;
    public GameObject Gray;
    public GameObject Pink;
    public GameObject Lime;
    public GameObject Orange;

    [Header("Gán 9 Sprite tương ứng AI")]
    public Sprite BlueSprite;
    public Sprite CyanSprite;
    public Sprite CoralSprite;
    public Sprite BrownSprite;
    public Sprite PurpleSprite;
    public Sprite GraySprite;
    public Sprite PinkSprite;
    public Sprite LimeSprite;
    public Sprite OrangeSprite;

    [Header("Odd Images (lẻ) - OddImages[0] là center")]
    public Image[] OddImages; // Size = 9

    [Header("Even Images (chẵn)")]
    public Image[] EvenImages; // Size = 8

    [Header("Panel chứa các image hiển thị")]
    public GameObject DeathDisplayPanel;

    [Header("Bản sao tiếp theo (phase kế tiếp)")]
    public GameObject nextPhaseObject; // Duplicate script ẩn sẵn, chỉ bật khi tới lượt

    private Dictionary<GameObject, Sprite> aiToSprite;
    private List<Sprite> recentDeaths = new();
    private HashSet<string> permanentlyDead = new();
    private bool hasPanelActivated = false;

    void Start()
    {
        Debug.Log($"[DeathDisplay] {gameObject.name} đã khởi động");

        aiToSprite = new Dictionary<GameObject, Sprite>
        {
            { Blue, BlueSprite },
            { Cyan, CyanSprite },
            { Coral, CoralSprite },
            { Brown, BrownSprite },
            { Purple, PurpleSprite },
            { Gray, GraySprite },
            { Pink, PinkSprite },
            { Lime, LimeSprite },
            { Orange, OrangeSprite }
        };

        // Ghi nhận trạng thái ban đầu
        foreach (var ai in aiToSprite.Keys)
        {
            if (ai == null) continue;

            string name = ai.name;

            if (!AIStateTracker.PreviousStates.ContainsKey(name))
                AIStateTracker.PreviousStates[name] = ai.activeSelf;

            // Nếu ngay từ đầu đã chết -> ghi nhận luôn
            if (!ai.activeSelf)
            {
                permanentlyDead.Add(name);
                recentDeaths.Add(aiToSprite[ai]);
            }
        }

        HideAllImages();
    }

    void Update()
    {
        CheckNewDeaths();
        CheckPanelActivation();
    }

    private void CheckNewDeaths()
    {
        foreach (var pair in aiToSprite)
        {
            GameObject ai = pair.Key;
            if (ai == null) continue;

            string name = ai.name;
            bool wasAlive = AIStateTracker.PreviousStates.ContainsKey(name) ? AIStateTracker.PreviousStates[name] : true;
            bool isAliveNow = ai.activeSelf;

            if (wasAlive && !isAliveNow && !permanentlyDead.Contains(name))
            {
                permanentlyDead.Add(name);
                recentDeaths.Add(pair.Value);
                Debug.Log($"[DeathDisplay] {name} vừa bị giết");
            }

            AIStateTracker.PreviousStates[name] = isAliveNow;
        }
    }

    private void CheckPanelActivation()
    {
        if (!hasPanelActivated && DeathDisplayPanel.activeSelf)
        {
            hasPanelActivated = true;
            DisplayRecentDeaths();
        }

        if (hasPanelActivated && !DeathDisplayPanel.activeSelf)
        {
            hasPanelActivated = false;
            HideAllImages();
        }
    }

    private void DisplayRecentDeaths()
    {
        int count = recentDeaths.Count;
        if (count == 0)
        {
            Debug.Log($"[DeathDisplay] {gameObject.name} - Không có ai chết.");
            GoToNextPhase();
            return;
        }

        HideAllImages();

        if (count % 2 == 1) // Lẻ
        {
            OddImages[0].sprite = recentDeaths[0];
            OddImages[0].gameObject.SetActive(true);

            for (int i = 1; i < count && i < OddImages.Length; i++)
            {
                OddImages[i].sprite = recentDeaths[i];
                OddImages[i].gameObject.SetActive(true);
            }
        }
        else // Chẵn
        {
            for (int i = 0; i < count && i < EvenImages.Length; i++)
            {
                EvenImages[i].sprite = recentDeaths[i];
                EvenImages[i].gameObject.SetActive(true);
            }
        }

        GoToNextPhase();
    }

    private void GoToNextPhase()
    {
        if (nextPhaseObject != null)
        {
            nextPhaseObject.SetActive(true);
            Debug.Log($"[DeathDisplay] {gameObject.name} kích hoạt {nextPhaseObject.name}");
        }

        Destroy(this); // Chỉ hủy script
    }

    private void HideAllImages()
    {
        foreach (var img in OddImages)
            if (img != null) img.gameObject.SetActive(false);

        foreach (var img in EvenImages)
            if (img != null) img.gameObject.SetActive(false);
    }
}
