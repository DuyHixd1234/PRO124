using UnityEngine;

public class PingFollow : MonoBehaviour
{
    [Header("References")]
    public Transform impostor;      // Player Impostor
    public Transform target;        // Crewmate để ping
    public Camera mainCamera;       // Gán Cinemachine Camera

    [Header("UI Settings")]
    public float screenEdgeOffset = 30f; // Khoảng cách lệch từ viền

    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (target == null || impostor == null || mainCamera == null || rectTransform == null || canvas == null) return;

        // Vector hướng từ impostor -> crewmate
        Vector3 dir = (target.position - impostor.position).normalized;

        // Lấy điểm xa xa theo hướng đó để camera nhìn thấy
        Vector3 worldPoint = impostor.position + dir * 50f;

        // Đổi sang viewport
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPoint);

        // Nếu ở sau camera -> lật lại
        if (viewportPos.z < 0)
        {
            viewportPos.x = 1f - viewportPos.x;
            viewportPos.y = 1f - viewportPos.y;
            viewportPos.z = 0;
        }

        // Ép vào trong màn hình (0..1)
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0f + screenEdgeOffset / Screen.width, 1f - screenEdgeOffset / Screen.width);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0f + screenEdgeOffset / Screen.height, 1f - screenEdgeOffset / Screen.height);

        // Chuyển về pixel
        Vector3 screenPos = mainCamera.ViewportToScreenPoint(viewportPos);

        // Đặt ping lên canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        rectTransform.localPosition = localPos;
    }
}
