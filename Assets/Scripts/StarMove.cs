using UnityEngine;
using UnityEngine.UI;

public class StarMove : MonoBehaviour
{
    [Header("Tốc độ di chuyển (sẽ được random từ spawner)")]
    public float moveSpeed = 30f;

    [Header("Hướng di chuyển (phải)")]
    public Vector2 direction = Vector2.right;

    [Header("Giá trị X vượt quá thì sẽ bị destroy")]
    public float outOfScreenX = 1200f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("❌ RectTransform không tồn tại!");
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Di chuyển sang phải, gấp đôi tốc độ
        rectTransform.anchoredPosition += direction * moveSpeed * 2f * Time.deltaTime;

        // Nếu vượt ra ngoài Canvas bên phải thì huỷ
        if (rectTransform.anchoredPosition.x > outOfScreenX)
        {
            Destroy(gameObject);
        }
    }
}
