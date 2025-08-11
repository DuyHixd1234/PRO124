using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSeeker : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 movement;

    [Header("UI")]
    public Canvas canvasGamePlay;       // Canvas chính (UI)
    public Canvas canvasVisionLimit;    // Canvas giới hạn tầm nhìn (nút Kill)
    public Slider cooldownSlider;       // Slider cooldown Kill
    public TextMeshProUGUI timerText;   // Thời gian đếm ngược Kill

    [Header("Kill Settings")]
    public Button killButton;           // Nút Kill
    public GameObject killRange;        // Vùng trigger phát hiện Crewmate
    public GameObject killEffect;       // GameObject kill bật tạm khi click
    public float killCooldown = 3f;     // Thời gian chờ sau Kill
    private bool canKill = true;

    private bool targetInRange = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Kill button mặc định tắt
        killButton.interactable = false;
        killButton.onClick.AddListener(OnKillButtonPressed);

        // Setup UI
        cooldownSlider.maxValue = killCooldown;
        cooldownSlider.value = 0;
        timerText.text = "";
    }

    void Update()
    {
        // Điều khiển di chuyển
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        anim.SetBool("isRunning", movement != Vector2.zero);

        if (movement.x < -0.1f) sr.flipX = true;
        else if (movement.x > 0.1f) sr.flipX = false;

        // Cập nhật trạng thái nút kill
        killButton.interactable = targetInRange && canKill;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnKillButtonPressed()
    {
        if (!canKill) return;

        StartCoroutine(KillRoutine());
    }

    IEnumerator KillRoutine()
    {
        canKill = false;
        killButton.interactable = false;

        // Hiệu ứng kill bật 0.1s
        if (killEffect != null)
        {
            killEffect.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            killEffect.SetActive(false);
        }

        // Cooldown UI
        float timeLeft = killCooldown;
        while (timeLeft > 0)
        {
            cooldownSlider.value = timeLeft;
            timerText.text = timeLeft.ToString("F1") + "s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        cooldownSlider.value = 0;
        timerText.text = "";
        canKill = true;
    }

    // Hàm này gọi từ KillRangeTrigger script
    public void SetTargetInRange(bool inRange)
    {
        targetInRange = inRange;
    }
}
