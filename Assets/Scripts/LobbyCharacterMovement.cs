using UnityEngine;

public class LobbyCharacterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Joystick Movement")]
    public JoystickController moveJoystick; // Gán joystick trái ở đây

    [Header("Laptop Proximity")]
    public PlayerLobbyController lobbyController;

    [Header("Footstep Sound")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.2f;
    private float footstepTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Lấy input từ joystick thay vì bàn phím
        if (moveJoystick != null)
        {
            movement = moveJoystick.Direction; // Vector2 từ joystick
        }
        else
        {
            movement = Vector2.zero;
        }

        // Animation chạy
        anim.SetBool("isRunning", movement.sqrMagnitude > 0.01f);

        // Lật nhân vật theo hướng di chuyển
        if (movement.x < -0.1f) sr.flipX = true;
        else if (movement.x > 0.1f) sr.flipX = false;

        // Xử lý âm thanh bước chân
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleFootsteps()
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayRandomFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // reset khi đứng yên
        }
    }

    void PlayRandomFootstep()
    {
        if (footstepClips.Length == 0 || audioSource == null) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laptop"))
        {
            lobbyController.SetLaptopProximity(true);
            Debug.Log("Da vao gan laptop");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Laptop"))
        {
            lobbyController.SetLaptopProximity(false);
            Debug.Log("Roi xa laptop");
        }
    }
}
