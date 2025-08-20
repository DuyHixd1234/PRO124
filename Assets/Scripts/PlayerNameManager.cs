using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerNameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nameInputField;   // Gán InputField
    public TMP_Text placeholderText;        // Gán Placeholder Text

    private const string playerNameKey = "PlayerName";
    private const string defaultName = "Player";

    void Start()
    {
        StartCoroutine(LoadNameRoutine());
    }

    IEnumerator LoadNameRoutine()
    {
        // Chờ 1 frame để chắc chắn UI đã khởi tạo
        yield return null;

        string savedName = PlayerPrefs.GetString(playerNameKey, "");

        if (string.IsNullOrEmpty(savedName))
        {
            // Chưa có tên → lưu Player 1 lần duy nhất
            PlayerPrefs.SetString(playerNameKey, defaultName);
            PlayerPrefs.Save();
            Debug.Log("[PlayerNameManager] Chưa có tên, set mặc định: " + defaultName);

            // Placeholder hiển thị Player
            if (placeholderText != null)
                placeholderText.text = defaultName;

            // InputField để trống, không đè lên
            nameInputField.text = "";
        }
        else
        {
            // Có sẵn tên cũ → lấy tên đó vào Placeholder
            Debug.Log("[PlayerNameManager] Đã tìm thấy tên cũ trong PlayerPrefs: " + savedName);

            if (placeholderText != null)
                placeholderText.text = savedName;

            // InputField để trống, để placeholder hiển thị
            nameInputField.text = "";
        }

        // Cập nhật liên tục mỗi frame (nếu bạn muốn sync liên tục)
        StartCoroutine(UpdatePlaceholderEveryFrame());
    }

    IEnumerator UpdatePlaceholderEveryFrame()
    {
        while (true)
        {
            string currentName = PlayerPrefs.GetString(playerNameKey, defaultName);
            if (placeholderText != null && !string.IsNullOrEmpty(currentName))
                placeholderText.text = currentName;

            yield return null; // check mỗi frame
        }
    }

    public void SavePlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        PlayerPrefs.SetString(playerNameKey, newName);
        PlayerPrefs.Save();
        Debug.Log("[PlayerNameManager] Đã lưu tên mới: " + newName);
    }
}
