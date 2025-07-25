using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIMove : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float killCooldown = 20f;
    public float detectRange = 0.5f;

    [Header("Body Sprites")]
    public Sprite bodyStandingSprite;
    public Sprite bodyLyingSprite;

    [Header("Body Display")]
    public GameObject bodyHolder; // GameObject con chua SpriteRenderer
    public SpriteRenderer bodyRenderer; // SpriteRenderer cua bodyHolder

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private Transform currentWaypoint;
    private Transform previousWaypoint;

    private bool isImpostor;
    private float killTimer;
    private bool gameStarted = true;
    private bool isDoingTask = false;
    private float taskTimer = 0f;
    private bool canMove = false;
    public bool isDead = false;

    void OnEnable()
    {
        SceneIntroFade.OnFadeDone += EnableAIMove;
    }

    void OnDisable()
    {
        SceneIntroFade.OnFadeDone -= EnableAIMove;
    }

    void EnableAIMove()
    {
        canMove = true;
    }

    public void StartTask(float duration)
    {
        isDoingTask = true;
        taskTimer = duration;
        anim.SetBool("isRunning", false);
        rb.linearVelocity = Vector2.zero;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        int aiIndex = GetAIIndexFromName();
        isImpostor = PlayerPrefs.GetInt($"AI_{aiIndex}_IsImpostor", 0) == 1;

        currentWaypoint = FindInitialWaypoint();
        StartCoroutine(StartGameAfterDelay());

        if (bodyHolder != null) bodyHolder.SetActive(false);
    }

    IEnumerator StartGameAfterDelay()
    {
        yield return new WaitForSeconds(20f);
        gameStarted = true;
    }

    void Update()
    {
        if (!canMove || isDead || currentWaypoint == null) return;

        if (isImpostor)
        {
            killTimer -= Time.deltaTime;
            TryKillNearbyCrewmate();
        }

        if (isDoingTask)
        {
            taskTimer -= Time.deltaTime;
            if (taskTimer <= 0f)
            {
                isDoingTask = false;
                anim.SetBool("isRunning", true);
            }
            return;
        }

        Vector2 direction = ((Vector2)currentWaypoint.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        if (direction.x > 0.1f) sr.flipX = false;
        else if (direction.x < -0.1f) sr.flipX = true;

        anim.SetBool("isRunning", true);

        float dist = Vector2.Distance(rb.position, currentWaypoint.position);
        if (dist < 0.1f)
        {
            previousWaypoint = currentWaypoint;
            currentWaypoint = GetNextWaypoint(currentWaypoint, previousWaypoint);

            if (currentWaypoint == null)
                anim.SetBool("isRunning", false);
        }
    }

    Transform GetNextWaypoint(Transform current, Transform previous)
    {
        Waypoint wp = current.GetComponent<Waypoint>();
        if (wp == null || wp.nextWaypoints.Length == 0) return null;

        Transform[] filtered = System.Array.FindAll(wp.nextWaypoints, t => t != previous);
        if (filtered.Length == 0) filtered = wp.nextWaypoints;

        return filtered[Random.Range(0, filtered.Length)];
    }

    Transform FindInitialWaypoint()
    {
        Waypoint[] all = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (Waypoint wp in all)
        {
            float dist = Vector2.Distance(transform.position, wp.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = wp.transform;
            }
        }
        return nearest;
    }

    void TryKillNearbyCrewmate()
    {
        if (killTimer > 0 || isDead) return;

        GameObject[] others = GameObject.FindGameObjectsWithTag("Crewmate");
        foreach (GameObject target in others)
        {
            if (target == gameObject) continue;

            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist < detectRange)
            {
                Debug.Log($"{gameObject.name} killed {target.name}");

                // === Danh dau la Body ===
                target.tag = "Body";

                AIMove victimAI = target.GetComponent<AIMove>();
                if (victimAI != null)
                {
                    victimAI.isDead = true;

                    // Tat hoan toan Animator + dung di chuyen
                    if (victimAI.anim != null)
                        victimAI.anim.enabled = false;

                    if (victimAI.rb != null)
                    {
                        victimAI.rb.linearVelocity = Vector2.zero;
                        victimAI.rb.bodyType = RigidbodyType2D.Kinematic;
                    }

                 
                    Collider2D col = target.GetComponent<Collider2D>();
                    if (col != null) col.enabled = false;

                   
                    if (victimAI.sr != null)
                        victimAI.sr.enabled = false;

                   
                    if (victimAI.bodyHolder != null && victimAI.bodyRenderer != null)
                    {
                        victimAI.bodyHolder.SetActive(true);
                        victimAI.bodyRenderer.sortingLayerName = "Body";
                        victimAI.bodyRenderer.sprite = victimAI.bodyStandingSprite;
                        victimAI.StartCoroutine(victimAI.StandThenFall());
                    }
                }

                killTimer = killCooldown;
                CheckWinLose();
                break;
            }
        }
    }


    public IEnumerator StandThenFall()
    {
        yield return new WaitForSeconds(1f);
        if (bodyRenderer != null)
        {
            bodyRenderer.sprite = bodyLyingSprite;
        }
    }


    void CheckWinLose()
    {
        int countImp = 0;
        int countCrewAlive = 0;

        GameObject[] all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in all)
        {
            if (!go.activeInHierarchy) continue;

            AIMove ai = go.GetComponent<AIMove>();
            if (ai == null) continue;

            if (ai.isDead) continue;

            if (go.tag == "Impostor") countImp++;
            else if (go.tag == "Crewmate") countCrewAlive++;
        }

        if (countImp == 0)
        {
            Debug.Log("All impostors defeated. You win!");
            SceneManager.LoadScene("Win");
        }
        else if (countImp >= countCrewAlive && countCrewAlive > 0)
        {
            Debug.Log("Too many impostors. You lose!");
            SceneManager.LoadScene("Lose");
        }
    }

    int GetAIIndexFromName()
    {
        string name = gameObject.name;
        if (name.StartsWith("AI_"))
        {
            string indexStr = name.Substring(3);
            int.TryParse(indexStr, out int index);
            return index;
        }
        return 0;
    }
}
