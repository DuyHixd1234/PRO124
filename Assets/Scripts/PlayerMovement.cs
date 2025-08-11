using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public JoystickController joystick; // Gán joystick trong Inspector
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        // Lấy input từ joystick (nếu tồn tại)
        if (joystick != null)
        {
            moveInput = joystick.Direction;
            Debug.Log($"[PlayerMovement] Joystick input: {moveInput}");
        }

        // Nếu joystick đang không dùng, lấy input từ bàn phím
        if (moveInput == Vector2.zero)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            if (moveInput != Vector2.zero)
                Debug.Log($"[PlayerMovement] Keyboard input: {moveInput}");
        }

        // Chuẩn hóa
        moveInput = moveInput.normalized;

        // Di chuyển
        rb.linearVelocity = moveInput * moveSpeed;
        Debug.Log($"[PlayerMovement] Set velocity: {rb.linearVelocity}");
    }
}
