using UnityEngine;
using TMPro;

public class NameLoader : MonoBehaviour
{
    public TMP_InputField playerNameInput;

    void Start()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            playerNameInput.text = savedName;  // Hiển thị lại tên đã lưu
        }
    }

    public void SaveName()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInput.text);
        PlayerPrefs.Save();
    }
}
