using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform outerCircle;  // Vòng ngoài
    public RectTransform handle;       // Cục trung tâm
    public float handleLimit = 100f;   // Giới hạn bán kính

    private Vector2 inputVector;
    public Vector2 Direction => inputVector; // Hướng di chuyển (-1..1)

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

        // Trả về hướng -1..1 thay vì pixel
        inputVector = pos / handleLimit;

        // Debug trạng thái
        //Debug.Log($"Joystick Direction: {inputVector}, Magnitude: {inputVector.magnitude}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
     //   Debug.Log("Joystick Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.localPosition = Vector2.zero;
        inputVector = Vector2.zero;
     //  Debug.Log("Joystick Released");
    }
}
