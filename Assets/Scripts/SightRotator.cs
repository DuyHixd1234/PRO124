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
        // Nếu joystick trái đang điều khiển nhân vật và joystick phải KHÔNG có input
        // => Không cho phép điều khiển bằng chuột/touch để tránh giật khi bấm Kill
        if (leftJoystick != null && leftJoystick.Direction.sqrMagnitude > 0.01f &&
            (rightJoystick == null || rightJoystick.Direction.sqrMagnitude <= 0.01f))
        {
            isRightClickHeld = false;
            return;
        }

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
            // Joystick phải ưu tiên hơn joystick trái
            targetDirection = rightJoystick.Direction.normalized;
            lastAimDir = targetDirection;
        }
        else if (leftJoystick != null && leftJoystick.Direction.sqrMagnitude > 0.01f)
        {
            // Chỉ khi joystick phải không hoạt động thì joystick trái mới xoay sight
            targetDirection = leftJoystick.Direction.normalized;
            lastAimDir = targetDirection;
        }
        // Nếu không có input nào, giữ nguyên hướng cũ

        // Xoay mượt
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float currentAngle = sightTransform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        sightTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
