using UnityEngine;

public enum AIRole { Crewmate, Impostor }

public class AIBase : MonoBehaviour
{
    [Header("AI Identity")]
    public string aiName;
    public AIRole role;

    [Header("Waypoint Movement")]
    public Waypoint startingWaypoint;
    public float speed = 8f;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer sr;
    protected Waypoint currentWaypoint;

    [Header("Task")]
    protected bool isDoingTask = false;
    protected GameObject currentTask;

    [Header("Kill / Death")]
    public bool isDead = false;
    public Sprite deadBodySprite;
    public GameObject shadowObject;
    public GameObject bodyPrefab;
    protected float killCooldown = 20f;
    protected float killTimer = 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (startingWaypoint == null)
        {
            Debug.LogError($"[{aiName}] has no starting waypoint assigned!");
            enabled = false;
            return;
        }

        currentWaypoint = startingWaypoint;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (role == AIRole.Impostor)
            killTimer -= Time.deltaTime;

        if (!isDoingTask)
            MoveToCurrentWaypoint();

        if (anim != null)
            anim.SetBool("isRunning", rb.linearVelocity.magnitude > 0.1f);
    }

    protected void MoveToCurrentWaypoint()
    {
        if (currentWaypoint == null) return;

        Vector3 target = currentWaypoint.transform.position;
        Vector3 direction = (target - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (sr != null && direction.x != 0)
            sr.flipX = direction.x < 0;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Transform nextTransform = currentWaypoint.GetRandomNext();
            if (nextTransform != null)
            {
                Waypoint nextWaypoint = nextTransform.GetComponent<Waypoint>();
                if (nextWaypoint != null)
                {
                    currentWaypoint = nextWaypoint;
                   
                }
                else
                {
                    Debug.LogWarning($"[{aiName}] next transform has no Waypoint component.");
                    rb.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                Debug.LogWarning($"[{aiName}] reached waypoint with no next waypoints: {currentWaypoint.name}");
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (role == AIRole.Crewmate && other.CompareTag("Task"))
        {
            
            isDoingTask = true;
            currentTask = other.gameObject;
            rb.linearVelocity = Vector2.zero;
            Invoke(nameof(CompleteTask), 3f);
        }

        if (role == AIRole.Impostor && other.CompareTag("Crewmate"))
        {
            TryKill(other.gameObject);
        }

        if (role == AIRole.Crewmate && other.CompareTag("Body"))
        {
            Debug.Log($"[{aiName}] found Body!");
            AIManager.Instance?.TriggerDiscussion(other.transform.position);
        }
    }

    protected void CompleteTask()
    {
        if (currentTask != null)
        {
                
            Destroy(currentTask);
        }

        isDoingTask = false;
    }

    protected void TryKill(GameObject target)
    {
        if (killTimer > 0f) return;

        AIBase targetAI = target.GetComponent<AIBase>();
        if (targetAI == null || targetAI.isDead || targetAI.role != AIRole.Crewmate) return;

        Debug.Log($"[{aiName}] killed [{targetAI.aiName}]");
        targetAI.OnKilled();
        killTimer = killCooldown;
    }

    public void OnKilled()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (shadowObject != null)
            shadowObject.SetActive(false);

        if (bodyPrefab != null)
        {
            GameObject body = Instantiate(bodyPrefab, transform.position, Quaternion.identity);
            SpriteRenderer bodySR = body.GetComponent<SpriteRenderer>();
            if (bodySR != null && deadBodySprite != null)
                bodySR.sprite = deadBodySprite;
        }

        // Ghi lại trạng thái bị kill
        string name = aiName;
        for (int i = 0; i < 9; i++)
        {
            string storedName = PlayerPrefs.GetString($"Shuffle_Name_{i + 1}", "");
            if (storedName == name)
            {
                PlayerPrefs.SetInt($"AI_{i}_IsDead", 1);
                break;
            }
        }

        Debug.Log($"[{aiName}] is now DEAD");
    }

}
