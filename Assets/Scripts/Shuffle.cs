// SCRIPTS CHINH SUA: Shuffle.cs
// Muc tieu: Dam bao du lieu shuffle duoc luu hoan chinh sang scene Map de cac script khac co the truy cap (ten, sprite, mau, role, index...) cho 10 nguoi (1 player + 9 AI)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Shuffle : MonoBehaviour
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
    public GameObject crewmatePanel;
    public GameObject impostorPanel;
    public GameObject roleRevealCrewPanel;
    public GameObject roleRevealImpostorPanel;

    [Header("UI Images")]
    public Image playerCrewImage;
    public Image playerImpostorImage;
    public Image aiImpostorImage;
    public Image roleRevealCrewImage;
    public Image roleRevealImpostorImage;

    [Header("UI Text - Role Impostor")]
    public TMP_Text impostorNameText1;
    public TMP_Text impostorNameText2;

    [Header("Timing")]
    public float firstPanelDuration = 3f;
    public float rolePanelDuration = 3f;

    private Sprite playerSprite;
    private string[] allNames = new string[10];
    private Sprite[] allSprites = new Sprite[10];
    private bool[] isImpostor = new bool[10];

    void Start()
    {
        SetupPlayerSprite();
        SetupShuffle();
        SaveShuffleData();
        PrintResultToConsole();

        // Tat UI
        crewmatePanel?.SetActive(false);
        impostorPanel?.SetActive(false);
        roleRevealCrewPanel?.SetActive(false);
        roleRevealImpostorPanel?.SetActive(false);

        StartCoroutine(WaitForShhhAndShowUI());
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
        allNames[0] = PlayerData.Instance.playerName;
        allSprites[0] = playerSprite;

        for (int i = 0; i < 9; i++)
        {
            allNames[i + 1] = aiNames[i];
            allSprites[i + 1] = aiSprites[i];
        }

        bool isPlayerImpostor = Random.value < 0.5f;
        int imp1, imp2;

        if (isPlayerImpostor)
        {
            isImpostor[0] = true;
            do { imp2 = Random.Range(1, 10); } while (imp2 == 0);
            isImpostor[imp2] = true;
            imp1 = 0;
        }
        else
        {
            imp1 = Random.Range(1, 10);
            do { imp2 = Random.Range(1, 10); } while (imp2 == imp1);
            isImpostor[imp1] = true;
            isImpostor[imp2] = true;
        }
    }

    void SaveShuffleData()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetString($"Shuffle_Name_{i}", allNames[i]);
            PlayerPrefs.SetInt($"Shuffle_Role_{i}", isImpostor[i] ? 1 : 0);
            PlayerPrefs.SetString($"Shuffle_Sprite_{i}", allSprites[i].name);
        }
        PlayerPrefs.SetInt("Player_IsImpostor", isImpostor[0] ? 1 : 0);
        PlayerPrefs.SetString("Player_Sprite", allSprites[0].name);
        PlayerPrefs.Save();
    }

    void PrintResultToConsole()
    {
        for (int i = 0; i < 10; i++)
        {
            string role = isImpostor[i] ? "Impostor" : "Crewmate";
            Debug.Log($"[RESULT] {allNames[i]} - {role}");
        }
    }

    IEnumerator WaitForShhhAndShowUI()
    {
        while (!HandShhh.shhhFinished) yield return null;

        if (isImpostor[0])
        {
            impostorPanel?.SetActive(true);
            playerImpostorImage.sprite = playerSprite;
            for (int i = 1; i < 10; i++)
            {
                if (isImpostor[i])
                {
                    aiImpostorImage.sprite = allSprites[i];
                    impostorNameText1.text = allNames[0];
                    impostorNameText2.text = allNames[i];
                    break;
                }
            }
        }
        else
        {
            crewmatePanel?.SetActive(true);
            playerCrewImage.sprite = playerSprite;
        }

        Object.FindAnyObjectByType<BlackPanelFade>()?.StartFadeOut();
        Object.FindAnyObjectByType<DoorSlide>()?.StartSlide();

        yield return new WaitForSeconds(firstPanelDuration);

        impostorPanel?.SetActive(false);
        crewmatePanel?.SetActive(false);

        if (isImpostor[0])
        {
            roleRevealImpostorPanel?.SetActive(true);
            roleRevealImpostorImage.sprite = playerSprite;
        }
        else
        {
            roleRevealCrewPanel?.SetActive(true);
            roleRevealCrewImage.sprite = playerSprite;
        }

        yield return new WaitForSeconds(rolePanelDuration);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }
}
