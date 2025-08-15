using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Cài đặt")]
    public float moveSpeed = 2f;
    public string role; // "Crewmate" hoặc "Impostor"
    public GameObject crewmateUI;
    public GameObject impostorUI;
    public GameObject panelDeadReported;
    public GameObject panelVoting;
    public Transform startWaypoint;

    [Header("Animation")]
    public Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isDead = false;
    [HideInInspector] public Vector2 movement;

    [Header("Xác")]
    public Sprite spriteXacDung;
    public Sprite spriteXacNam;
    public GameObject bodyHolder; // GameObj riêng để chứa body
    public SpriteRenderer bodyRenderer;

    [Header("9 Canvas theo dõi")]
    public GameObject[] canvasesToWatch;

    [Header("Joystick (Tùy chọn)")]
    public JoystickController joystick; // Kéo thả JoystickController vào đây trong Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        int isImp = PlayerPrefs.GetInt("Player_IsImpostor", 0);
        role = isImp == 1 ? "Impostor" : "Crewmate";

        crewmateUI.SetActive(role == "Crewmate");
        impostorUI.SetActive(role == "Impostor");

        bodyHolder.SetActive(false); // Ẩn body lúc đầu
    }

    void Update()
    {
        if (isDead) return;

        // Kiểm tra nếu bất kỳ canvas nào bật → spawn về startWaypoint
        foreach (var canvasObj in canvasesToWatch)
        {
            if (canvasObj != null && canvasObj.activeSelf)
            {
                TeleportToStart();
                break;
            }
        }

        // --- Nhận input từ joystick hoặc bàn phím ---
        Vector2 joystickInput = joystick != null ? joystick.Direction : Vector2.zero;
        Vector2 keyboardInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Ưu tiên joystick nếu có input, nếu không dùng bàn phím
        if (joystickInput.magnitude > 0.1f)
            movement = joystickInput;
        else
            movement = keyboardInput;

        // Animation
        anim.SetBool("isRunning", movement != Vector2.zero);

        // Flip khi di chuyển trái/phải
        if (movement.x < -0.1f) sr.flipX = true;
        else if (movement.x > 0.1f) sr.flipX = false;
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void TeleportToStart()
    {
        transform.position = startWaypoint.position;
        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;
        anim.SetBool("isRunning", false);
    }

    public void OnDeadBodyReported()
    {
        StartCoroutine(HandleReport());
    }

    IEnumerator HandleReport()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isRunning", false);
        transform.position = startWaypoint.position;

        panelDeadReported.SetActive(true);
        yield return new WaitForSeconds(4f);
        panelDeadReported.SetActive(false);

        panelVoting.SetActive(true); // Show UI vote
    }

    public void KillByImpostor()
    {
        if (isDead || role != "Crewmate") return;
        StartCoroutine(HandleKilled());
    }

    IEnumerator HandleKilled()
    {
        isDead = true;

        // Tắt sprite và animation chính
        anim.enabled = false;
        sr.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // Show xác đứng
        bodyHolder.SetActive(true);
        bodyRenderer.sortingLayerName = "Character";
        bodyRenderer.sortingOrder = 1;
        bodyRenderer.sprite = spriteXacDung;

        yield return new WaitForSeconds(1f);

        // Hiện xác nằm
        bodyRenderer.sprite = spriteXacNam;
        bodyRenderer.sortingOrder = 5;

        yield return new WaitForSeconds(1f);

        // Thua cuộc
        Object.FindAnyObjectByType<BlackPanelFade>()?.StartFadeOut();
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Lose");
    }
}
