using UnityEngine;

public class Ghost : MonoBehaviour
{
    [Header("Cài đặt")]
    public float moveSpeed = 2f; // tốc độ di chuyển

    [Header("Tham chiếu Joystick (nếu có)")]
    public JoystickController joystick; // Gán trong Inspector

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Luôn để idle
        if (anim != null)
            anim.SetBool("isRunning", false);
    }

    void Update()
    {
        Vector2 joystickInput = Vector2.zero;

        // Nếu có joystick thì đọc hướng
        if (joystick != null)
        {
            joystickInput = joystick.Direction;
        }

        // Ưu tiên joystick nếu có input
        if (joystickInput.magnitude > 0.1f)
        {
            movement = joystickInput;
        }
        else
        {
            // Nếu joystick không dùng, lấy input bàn phím
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        // Lật sprite
        if (sr != null)
        {
            if (movement.x < -0.1f) sr.flipX = true;
            else if (movement.x > 0.1f) sr.flipX = false;
        }

        // Cập nhật animation
        if (anim != null)
        {
            anim.SetBool("isRunning", movement.magnitude > 0.1f);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
