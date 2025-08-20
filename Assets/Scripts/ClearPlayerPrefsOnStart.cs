using UnityEngine;

public class ClearPlayerPrefsOnStart : MonoBehaviour
{
    private const string nameKey = "PlayerName";

    void Start()
    {
        ClearExceptName();
    }

    void Update()
    {
        ClearExceptName();
    }

    void ClearExceptName()
    {
        // Lưu tên tạm
        string savedName = PlayerPrefs.GetString(nameKey, "Player");

        // Xoá tất cả
        PlayerPrefs.DeleteAll();

        // Ghi lại tên
        PlayerPrefs.SetString(nameKey, savedName);

        PlayerPrefs.Save();
    }
}
