using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLobbyController : MonoBehaviour
{
    [Header("References")]
    public GameObject redPlayer;
    public GameObject yellowPlayer;
    public GameObject greenPlayer;
    public GameObject whitePlayer;
    public GameObject laptopTriggerUI;
    public GameObject colorPanelUI;
    public Button useButton;
    public Button closeColorPanelButton;
    public GameObject mainUIPanel;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private bool isNearLaptop = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetColorFromPlayerData();

        useButton.interactable = false;
        colorPanelUI.SetActive(false);

        closeColorPanelButton.onClick.RemoveAllListeners();
        closeColorPanelButton.onClick.AddListener(() =>
        {
            colorPanelUI.SetActive(false);
            mainUIPanel.SetActive(true);
        });

        CanvasGroup cg = useButton.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0.6f;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }

    void Update()
    {
        UpdateNameTextPosition();
    }

    void UpdateNameTextPosition()
    {
        Vector3 offset = new Vector3(0, 1.5f, 0);
        // Tạm thời trống
    }

    public void SetLaptopProximity(bool near)
    {
        isNearLaptop = near;

        if (useButton != null)
        {
            useButton.interactable = near;

            CanvasGroup cg = useButton.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = near ? 1f : 0.6f;
        }
    }

    public void OnClickUse()
    {
        if (isNearLaptop)
        {
            mainUIPanel.SetActive(false);
            colorPanelUI.SetActive(true);
        }
    }

    public void SelectColor(int colorIndex)
    {
        // Luu vao PlayerData
        PlayerData.Instance.selectedColorIndex = colorIndex;

        // Tat het tat ca prefab truoc khi bat cai can chon
        DisableAllPlayers();

        // Bat prefab tuong ung
        switch (colorIndex)
        {
            case 0:
                redPlayer.SetActive(true);
                break;
            case 1:
                yellowPlayer.SetActive(true);
                break;
            case 2:
                greenPlayer.SetActive(true);
                break;
            case 3:
                whitePlayer.SetActive(true);
                break;
        }

        Debug.Log("Chon mau index: " + colorIndex);
    }

    void SetColorFromPlayerData()
    {
        int colorIndex = PlayerData.Instance.selectedColorIndex;

        DisableAllPlayers();

        switch (colorIndex)
        {
            case 0:
                redPlayer.SetActive(true);
                break;
            case 1:
                yellowPlayer.SetActive(true);
                break;
            case 2:
                greenPlayer.SetActive(true);
                break;
            case 3:
                whitePlayer.SetActive(true);
                break;
        }
    }

    void DisableAllPlayers()
    {
        redPlayer.SetActive(false);
        yellowPlayer.SetActive(false);
        greenPlayer.SetActive(false);
        whitePlayer.SetActive(false);
    }
}
