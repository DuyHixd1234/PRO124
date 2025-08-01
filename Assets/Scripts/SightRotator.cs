using UnityEngine;

public class SightRotator : MonoBehaviour
{
    [Header("Sight Settings")]
    public Transform sightTransform; // Kéo game object khung vuông vào đây
    public float rotateSpeed = 720f; // độ/giây

    private Vector2 lastMoveDir = Vector2.right; // Mặc định hướng phải
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
        Vector2 targetDirection = lastMoveDir;

        if (isRightClickHeld)
        {
            targetDirection = mouseTargetDirection;
        }
        else
        {
            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (moveInput.sqrMagnitude > 0.01f)
            {
                targetDirection = moveInput.normalized;
                lastMoveDir = targetDirection;
            }
        }

        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float currentAngle = sightTransform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        sightTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
