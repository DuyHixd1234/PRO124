using UnityEngine;

public class SightRotator : MonoBehaviour
{
    [Header("Sight Settings")]
    public Transform sightTransform;
    public float rotateSpeed = 720f;

    [Header("Joystick References")]
    public JoystickController leftJoystick;   // Joystick di chuyển
    public JoystickController rightJoystick;  // Joystick xoay sight

    private Vector2 lastAimDir = Vector2.right;
    private bool joystick2Active = false;

    void Update()
    {
        UpdateSightRotation();
    }

    void UpdateSightRotation()
    {
        Vector2 targetDirection = lastAimDir;

        // ƯU TIÊN JOYSTICK 2
        if (rightJoystick != null && rightJoystick.Direction.sqrMagnitude > 0.01f)
        {
            joystick2Active = true;
            targetDirection = rightJoystick.Direction.normalized;
            lastAimDir = targetDirection;

            Debug.Log($"[Sight] Joystick 2 điều khiển: {targetDirection}");
        }
        else
        {
            // Nếu Joystick 2 vừa nhả
            if (joystick2Active)
            {
                Debug.Log("[Sight] Joystick 2 thả → trả quyền lại cho Joystick 1");
                joystick2Active = false;
            }

            // CHỈ cho Joystick 1 điều khiển khi Joystick 2 KHÔNG active
            if (!joystick2Active && leftJoystick != null && leftJoystick.Direction.sqrMagnitude > 0.01f)
            {
                targetDirection = leftJoystick.Direction.normalized;
                lastAimDir = targetDirection;

                Debug.Log($"[Sight] Joystick 1 điều khiển: {targetDirection}");
            }
        }

        // Xoay sight
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float currentAngle = sightTransform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        sightTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
