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

    private Dictionary<GameObject, Sprite> aiToSprite;
    private Dictionary<GameObject, bool> currentStates = new();

    private List<Sprite> recentDeaths = new();
    private HashSet<string> permanentlyDead = new(); // Danh sách AI đã chết

    private bool hasPanelActivated = false;

    void Start()
    {
        Debug.Log("[DeathDisplay] Script đã khởi động");

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

        foreach (var ai in aiToSprite.Keys)
        {
            currentStates[ai] = ai.activeSelf;

            string name = ai.name;

            if (!AIStateTracker.PreviousStates.ContainsKey(name))
            {
                AIStateTracker.PreviousStates[name] = ai.activeSelf;
                Debug.Log($"[DeathDisplay] Lưu trạng thái ban đầu: {name} = {ai.activeSelf}");

                if (!ai.activeSelf)
                {
                    permanentlyDead.Add(name);
                    recentDeaths.Add(aiToSprite[ai]);
                    Debug.Log($"[DeathDisplay] Ghi nhận AI chết sẵn: {name}");
                }
            }
            else
            {
                Debug.Log($"[DeathDisplay] Đã có trạng thái cũ: {name} = {AIStateTracker.PreviousStates[name]}");
            }
        }

        HideAllImages();
    }

    void Update()
    {
        CheckNewDeaths(); // Luôn theo dõi kill
        CheckPanelActivation(); // Chờ panel bật lên để show
    }

    private void CheckNewDeaths()
    {
        foreach (var pair in aiToSprite)
        {
            GameObject ai = pair.Key;
            string name = ai.name;

            bool wasAlive = AIStateTracker.PreviousStates.ContainsKey(name) ? AIStateTracker.PreviousStates[name] : true;
            bool isAliveNow = ai.activeSelf;

            if (wasAlive && !isAliveNow && !permanentlyDead.Contains(name))
            {
                permanentlyDead.Add(name);
                recentDeaths.Add(pair.Value);
                Debug.Log($"[DeathDisplay] AI {name} vừa bị giết");
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
            Debug.Log("[DeathDisplay] Không có ai chết trong pha này.");
            return;
        }

        Debug.Log($"[DeathDisplay] Có {count} AI chết trong pha này.");

        HideAllImages();

        if (count % 2 == 1) // Lẻ
        {
            // Hiện center trước
            OddImages[0].sprite = recentDeaths[0];
            OddImages[0].gameObject.SetActive(true);

            // Các ảnh còn lại
            for (int i = 1; i < count && i < OddImages.Length; i++)
            {
                Image img = OddImages[i];
                img.sprite = recentDeaths[i];
                img.gameObject.SetActive(true);
            }
        }
        else // Chẵn
        {
            // KHÔNG hiện OddImages[0] nữa!
            for (int i = 0; i < count && i < EvenImages.Length; i++)
            {
                Image img = EvenImages[i];
                img.sprite = recentDeaths[i];
                img.gameObject.SetActive(true);
            }
        }
    }


    private void HideAllImages()
    {
        foreach (var img in OddImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (var img in EvenImages)
        {
            img.gameObject.SetActive(false);
        }
    }
}
