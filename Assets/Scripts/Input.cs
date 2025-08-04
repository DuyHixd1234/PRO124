using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuFlowManager : MonoBehaviour
{
    [Header("UI")]
    public Button playButton;
    public GameObject startPanel;
    public TMP_InputField nameInput;
    public MenuManager menuManager;

    private bool inputStarted = false;

    void Start()
    {
        startPanel.SetActive(false);
        playButton.onClick.AddListener(OnPlayClicked);

        // Gắn sự kiện Submit (Enter)
        nameInput.onSubmit.AddListener(OnNameSubmit);
    }

    void Update()
    {
        if (!inputStarted) return;

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

    void OnPlayClicked()
    {
        Debug.Log("Đã click nút Play, bật panel nhập tên.");
        startPanel.SetActive(true);
        inputStarted = true;
        nameInput.ActivateInputField(); // Focus input
    }

    void OnNameSubmit(string inputText)
    {
        Debug.Log("Đã nhấn Enter (Submit) trong InputField");

        if (menuManager != null && inputText.Trim().Length > 0)
        {
            Debug.Log("Tên hợp lệ, bắt đầu game qua Enter (Submit)");
            menuManager.OnStartGame();
        }
        else
        {
            Debug.Log("Tên rỗng, không bắt đầu");
        }
    }
}
