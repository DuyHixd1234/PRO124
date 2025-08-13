using UnityEngine;

public class ClearPlayerPrefsOnStart : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[PlayerPrefs] Dữ liệu PlayerPrefs đã được xóa hoàn toàn khi bắt đầu game.");
    }
    void Update()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    
    }
}
