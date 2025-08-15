using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f; // tốc độ di chuyển
    public Animator animator;    // Animator để bật tắt isRunning

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Lấy input từ ASDW và phím mũi tên
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        // Animation
        bool isRunning = moveInput.sqrMagnitude > 0;
        animator.SetBool("isRunning", isRunning);

        // Lật nhân vật theo hướng di chuyển
        if (moveX > 0 && !facingRight)
            Flip();
        else if (moveX < 0 && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        // Di chuyển
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
