using UnityEngine;
using TMPro;

public class MenuFlowManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startPanel;
    public TMP_InputField nameInput;
    public MenuManager menuManager;

    private const string playerNameKey = "PlayerName";
    private const string defaultName = "Player";

    void Start()
    {
        // Bật panel nhập tên luôn
        startPanel.SetActive(true);

        // Load tên đã lưu hoặc đặt mặc định
        string savedName = PlayerPrefs.GetString(playerNameKey, defaultName);
        nameInput.text = savedName;

        // Đăng ký sự kiện Submit (Enter)
        nameInput.onSubmit.AddListener(OnNameSubmit);
    }

    void Update()
    {
        // Giới hạn tên 20 ký tự (không tính dấu cách)
        string raw = nameInput.text;
        string noSpace = raw.Replace(" ", "");
        if (noSpace.Length > 20)
        {
            int validCount = 0;
            string result = "";
            foreach (char c in raw)
            {
                if (c != ' ') validCount++;
                if (validCount > 20) break;
                result += c;
            }
            nameInput.text = result;
            nameInput.caretPosition = result.Length;
        }
    }

    void OnNameSubmit(string inputText)
    {
        string finalName = inputText.Trim();

        // Nếu người chơi không nhập, dùng tên cũ hoặc mặc định
        if (string.IsNullOrEmpty(finalName))
        {
            finalName = PlayerPrefs.GetString(playerNameKey, defaultName);
            nameInput.text = finalName;
        }

        // Lưu vào PlayerPrefs
        PlayerPrefs.SetString(playerNameKey, finalName);
        PlayerPrefs.Save();

        Debug.Log($"Tên đã lưu: {finalName}");

        // Bắt đầu game
        if (menuManager != null)
        {
            menuManager.OnStartGame();
        }
    }
}
