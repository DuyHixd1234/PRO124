using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSelectorUI : MonoBehaviour
{
    [System.Serializable]
    public class ColorOption
    {
        public int colorIndex;
        public Button button;
        public GameObject tick;           // Dấu tick hiển thị trên nút
        public TMP_Text label;            // Label mô tả
        public Sprite spritePreview;      // Sprite preview cho image chính
    }

    [Header("UI Elements")]
    public ColorOption[] colorOptions;
    public Image previewImage;            // Ảnh chính hiển thị sprite

    [Header("References")]
    public PlayerLobbyController lobbyController;   // Gán từ inspector

    void Start()
    {
        foreach (ColorOption opt in colorOptions)
        {
            int index = opt.colorIndex;
            opt.button.onClick.AddListener(() => OnSelectColor(index));
        }

        // Load mau hien tai
        int savedIndex = PlayerData.Instance.selectedColorIndex;
        UpdateUI(savedIndex);

        // Cap nhat prefab cho lobby ngay tu dau
        if (lobbyController != null)
            lobbyController.SelectColor(savedIndex);
    }

    void OnSelectColor(int selectedIndex)
    {
        PlayerData.Instance.selectedColorIndex = selectedIndex;

        UpdateUI(selectedIndex);

        if (lobbyController != null)
            lobbyController.SelectColor(selectedIndex);

        Debug.Log("Selected color: " + selectedIndex);
    }

    void UpdateUI(int selectedIndex)
    {
        foreach (ColorOption opt in colorOptions)
        {
            bool isSelected = (opt.colorIndex == selectedIndex);

            if (opt.tick != null)
                opt.tick.SetActive(isSelected);

            if (opt.label != null)
                opt.label.gameObject.SetActive(isSelected);

            if (isSelected && previewImage != null)
                previewImage.sprite = opt.spritePreview;
        }
    }
}
