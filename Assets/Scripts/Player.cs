using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Cai dat")]
    public float moveSpeed = 2f;
    public string role; // "Crewmate" hoac "Impostor"
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
    private Vector2 movement;

    [Header("Xac")]
    public Sprite spriteXacDung;
    public Sprite spriteXacNam;
    public GameObject bodyHolder; // GameObj rieng de chua body
    public SpriteRenderer bodyRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        int isImp = PlayerPrefs.GetInt("Player_IsImpostor", 0);
        role = isImp == 1 ? "Impostor" : "Crewmate";

        crewmateUI.SetActive(role == "Crewmate");
        impostorUI.SetActive(role == "Impostor");

        bodyHolder.SetActive(false); // An body luc dau
    }

    void Update()
    {
        if (isDead) return;

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        anim.SetBool("isRunning", movement != Vector2.zero);
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

        // Tat sprite va animation chinh
        anim.enabled = false;
        sr.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // Show xac dung
        bodyHolder.SetActive(true);
        bodyRenderer.sortingLayerName = "Character";
        bodyRenderer.sortingOrder = 1;
        bodyRenderer.sprite = spriteXacDung;

        yield return new WaitForSeconds(1f);

        // Hien xac nam
        bodyRenderer.sprite = spriteXacNam;
        bodyRenderer.sortingOrder = 5;

        yield return new WaitForSeconds(1f);

        // Thua cuoc
        Object.FindAnyObjectByType<BlackPanelFade>()?.StartFadeOut();
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Lose");
    }
}
