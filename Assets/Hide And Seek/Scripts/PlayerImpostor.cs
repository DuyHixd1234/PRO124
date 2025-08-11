using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerImpostor : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;
    private float currentSpeed;

    [Header("References")]
    public JoystickController joystick; // 🔹 Thêm để gán joystick
    public Slider speedBoostSlider;
    public Canvas killCanvas;
    public Transform spawnWaypoint;
    public Animator animator;
    public Button killButton;
    public TMP_Text cooldownText;

    [Header("Kill Mechanics")]
    public GameObject checkRangeObj;   // CircleCollider2D isTrigger = true
    public GameObject killRangeObj;    // Ẩn sẵn

    private bool canKill = false;
    private bool isCooldown = false;
    private float cooldownDuration = 1f;
    private float cooldownTimer = 0f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentSpeed = baseSpeed;

        if (spawnWaypoint != null)
            transform.position = spawnWaypoint.position;

        if (killCanvas != null)
            killCanvas.gameObject.SetActive(true);

        if (speedBoostSlider != null)
            speedBoostSlider.gameObject.SetActive(false);

        if (killRangeObj != null)
            killRangeObj.SetActive(false);

        if (killButton != null)
        {
            killButton.interactable = false;
            killButton.onClick.AddListener(OnKillButtonClicked);
        }

        UpdateCooldownText(0);
    }

    void Update()
    {
        Vector2 joystickInput = Vector2.zero;

        // 🔹 Lấy input joystick nếu có
        if (joystick != null)
            joystickInput = joystick.Direction;

        // 🔹 Ưu tiên joystick nếu có input
        if (joystickInput.magnitude > 0.1f)
        {
            movement = joystickInput.normalized;
        }
        else
        {
            // Nếu joystick đứng yên, lấy input bàn phím
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        // Kiểm tra speed boost
        currentSpeed = (speedBoostSlider != null && speedBoostSlider.gameObject.activeSelf)
            ? baseSpeed + 3f
            : baseSpeed;

        // Kiểm tra cooldown
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isCooldown = false;
                cooldownTimer = 0f;
                UpdateCooldownText(0);
            }
            else
            {
                UpdateCooldownText(Mathf.CeilToInt(cooldownTimer));
            }
        }

        // 🔹 Check va chạm crewmate mỗi frame
        CheckCrewmateProximity();

        // Kill bằng phím K
        if (Input.GetKeyDown(KeyCode.K) && !isCooldown && canKill)
        {
            DoKill();
        }

        // Animator chạy
        if (animator != null)
            animator.SetBool("isRunning", movement.sqrMagnitude > 0.01f);

        // Lật sprite
        if (sr != null)
        {
            if (movement.x < -0.1f) sr.flipX = true;
            else if (movement.x > 0.1f) sr.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
            rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    // 🔹 Hàm kiểm tra liên tục crewmate trong vùng check
    private void CheckCrewmateProximity()
    {
        canKill = false;

        if (checkRangeObj != null)
        {
            Collider2D checkCollider = checkRangeObj.GetComponent<Collider2D>();
            if (checkCollider != null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(checkCollider.bounds.center, checkCollider.bounds.extents.x);
                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Crewmate") && hit.gameObject != gameObject)
                    {
                        canKill = true;
                        break;
                    }
                }
            }
        }

        // Chỉ cho phép bấm nút khi có crewmate và không cooldown
        if (killButton != null)
            killButton.interactable = canKill && !isCooldown;
    }

    private void OnKillButtonClicked()
    {
        if (!isCooldown && canKill)
            DoKill();
    }

    private void DoKill()
    {
        if (killRangeObj == null) return;

        killRangeObj.SetActive(true);

        isCooldown = true;
        cooldownTimer = cooldownDuration;

        killButton.interactable = false;

        Invoke(nameof(DisableKillRange), 0.5f);
    }

    private void DisableKillRange()
    {
        if (killRangeObj != null)
            killRangeObj.SetActive(false);
    }

    private void UpdateCooldownText(int time)
    {
        if (cooldownText != null)
            cooldownText.text = time > 0 ? time.ToString() : "";
    }
}
