using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickSightController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Elements")]
    public RectTransform outerCircle;  // Vòng ngoài
    public RectTransform handle;       // Cục trung tâm
    public float handleLimit = 100f;   // Giới hạn bán kính

    private Vector2 inputVector;
    public Vector2 Direction => inputVector; // Hướng xoay sight (-1..1)

    public bool IsActive => inputVector.sqrMagnitude > 0.01f;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            outerCircle,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos = Vector2.ClampMagnitude(pos, handleLimit);
        handle.localPosition = pos;

        // Hướng chuẩn hóa (-1..1)
        inputVector = pos / handleLimit;

        Debug.Log($"[Joystick 2] Đang xoay sight: {inputVector}, Magnitude: {inputVector.magnitude:F2}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        Debug.Log("[Joystick 2] Bắt đầu điều khiển sight");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.localPosition = Vector2.zero;
        inputVector = Vector2.zero;
        Debug.Log("[Joystick 2] Thả điều khiển sight");
    }
}
