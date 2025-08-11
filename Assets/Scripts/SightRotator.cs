using UnityEngine;

public class SightRotator : MonoBehaviour
{
    [Header("Sight Settings")]
    public Transform sightTransform;
    public float rotateSpeed = 720f;

    [Header("Joystick References")]
    public JoystickController leftJoystick;   // Joystick di chuyển
    public JoystickController rightJoystick;  // Joystick ngắm

    private Vector2 lastAimDir = Vector2.right;
    private bool isRightClickHeld = false;
    private Vector3 mouseTargetDirection;

    void Update()
    {
        HandleMouseOverride();
        UpdateSightRotation();
    }

    void HandleMouseOverride()
    {
        if (Input.GetMouseButton(1))
        {
            isRightClickHeld = true;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mouseWorldPos - transform.position;
            direction.z = 0;

            if (direction.magnitude > 0.01f)
            {
                mouseTargetDirection = direction.normalized;
            }
        }
        else
        {
            isRightClickHeld = false;
        }
    }

    void UpdateSightRotation()
    {
        Vector2 targetDirection = lastAimDir; // Mặc định giữ hướng cũ

        if (isRightClickHeld)
        {
            // Chuột phải ưu tiên
            targetDirection = mouseTargetDirection;
            lastAimDir = targetDirection;
        }
        else if (rightJoystick != null && rightJoystick.Direction.sqrMagnitude > 0.01f)
        {
            // Ưu tiên joystick phải để xoay
            targetDirection = rightJoystick.Direction.normalized;
            lastAimDir = targetDirection;
        }
        else if (leftJoystick != null && leftJoystick.Direction.sqrMagnitude > 0.01f)
        {
            // Nếu không dùng joystick phải, xoay theo joystick di chuyển
            targetDirection = leftJoystick.Direction.normalized;
            lastAimDir = targetDirection;
        }
        // Nếu không có input nào, giữ nguyên lastAimDir

        // Xoay mượt
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float currentAngle = sightTransform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        sightTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
