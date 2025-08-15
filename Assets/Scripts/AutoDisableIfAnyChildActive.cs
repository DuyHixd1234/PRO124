using UnityEngine;

public class AutoDisableIfAnyChildActive : MonoBehaviour
{
    [Header("Các GameObject cần kiểm tra")]
    public GameObject[] elementsToCheck;

    void Update()
    {
        foreach (var element in elementsToCheck)
        {
            if (element != null && element.activeSelf)
            {
                // Nếu 1 element đang active → tắt gameobject chứa script này
                gameObject.SetActive(false);
                return; // Thoát luôn để tránh lỗi khi object bị disable
            }
        }
    }
}
