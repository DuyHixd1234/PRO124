using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HSShuffle : MonoBehaviour
{
    [Header("Player Sprites")]
    public Sprite redPlayerSprite;
    public Sprite yellowPlayerSprite;
    public Sprite darkGreenPlayerSprite;
    public Sprite whitePlayerSprite;

    [Header("AI Sprites + Names")]
    public Sprite[] aiSprites = new Sprite[9];
    public string[] aiNames = new string[9];

    [Header("UI Panels")]
    public GameObject impostorPanel;         // Panel luôn hiển thị tên + sprite impostor
    public GameObject instructionPanel;      // Panel hướng dẫn chỉ hiện nếu Human là impostor
    public GameObject nonImpostorPanel;      // Panel cho crewmate nếu impostor là AI

    [Header("UI Images")]
    public Image impostorImage;

    [Header("UI Text")]
    public TMP_Text impostorNameText;

    [Header("Timing & Fade")]
    public float totalDuration = 6f;             // Tổng thời gian giữ panel impostor
    public GameObject finalObjectToEnable;       // Bật cái này ở giây cuối

    private Sprite playerSprite;
    private string[] allNames = new string[10];
    private Sprite[] allSprites = new Sprite[10];
    private bool[] isImpostor = new bool[10];
    private int impostorIndex = -1;

    void Start()
    {
        SetupPlayerSprite();
        SetupShuffle();
        SaveShuffleData();
        PrintResultToConsole();

        impostorPanel?.SetActive(false);
        instructionPanel?.SetActive(false);
        nonImpostorPanel?.SetActive(false);
        finalObjectToEnable?.SetActive(false);

        StartCoroutine(HandleUIFlow());
    }

    void SetupPlayerSprite()
    {
        int colorIndex = PlayerData.Instance.selectedColorIndex;
        switch (colorIndex)
        {
            case 0: playerSprite = redPlayerSprite; break;
            case 1: playerSprite = yellowPlayerSprite; break;
            case 2: playerSprite = darkGreenPlayerSprite; break;
            case 3: playerSprite = whitePlayerSprite; break;
            default: Debug.LogError("Invalid player color index!"); break;
        }
    }

    void SetupShuffle()
    {
        // Gán Human (index 0)
        allNames[0] = PlayerData.Instance.playerName;
        allSprites[0] = playerSprite;

        // Gán 9 AI (index 1–9)
        for (int i = 0; i < 9; i++)
        {
            allNames[i + 1] = aiNames[i];
            allSprites[i + 1] = aiSprites[i];
        }

        // Xác suất: Player 25%, AI 75% chia đều
        float roll = Random.value; // random 0.0 → 1.0
        if (roll < 0.25f)
        {
            impostorIndex = 0; // Player làm impostor
        }
        else
        {
            // Chọn 1 AI trong 1–9
            impostorIndex = Random.Range(1, 10);
        }

        isImpostor[impostorIndex] = true;
    }


    void SaveShuffleData()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetString($"Shuffle_Name_{i}", allNames[i]);
            PlayerPrefs.SetString($"Shuffle_Sprite_{i}", allSprites[i].name);
            PlayerPrefs.SetInt($"Shuffle_Role_{i}", isImpostor[i] ? 1 : 0);
            PlayerPrefs.SetInt($"Shuffle_Alive_{i}", 1);
        }

        PlayerPrefs.SetInt("HideSeek_ImpostorIndex", impostorIndex);
        PlayerPrefs.SetString("GameMode", "HideSeek");
        PlayerPrefs.Save();
    }

    void PrintResultToConsole()
    {
        for (int i = 0; i < 10; i++)
        {
            string role = isImpostor[i] ? "Impostor" : "Crewmate";
            Debug.Log($"[HIDE SEEK] {allNames[i]} - {role}");
        }
    }

    IEnumerator HandleUIFlow()
    {
        // Đợi panel Shhhh kết thúc
        while (!HandShhh.shhhFinished) yield return null;

        // Hiện panel Impostor
        impostorPanel?.SetActive(true);
        impostorImage.sprite = allSprites[impostorIndex];
        impostorNameText.text = allNames[impostorIndex];

        // Hiện panel hướng dẫn tương ứng
        if (impostorIndex == 0)
        {
            instructionPanel?.SetActive(true); // Người chơi là Impostor
        }
        else
        {
            nonImpostorPanel?.SetActive(true); // Người chơi là Crewmate
        }

        // Đợi tới thời điểm còn 1s
        yield return new WaitForSeconds(totalDuration - 1f);

        // Bật object ẩn ra
        finalObjectToEnable?.SetActive(true);

        // Đợi nốt 1 giây
        yield return new WaitForSeconds(1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("HSMap");
    }
}
