using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AIImpostorHideSeek : MonoBehaviour
{
    [Header("Di chuyển")]
    public float normalSeekSpeed = 5f;    // tốc độ lần seek 1
    public float finalSeekSpeed = 7f;     // tốc độ final seek
    public Animator animator;
    private SpriteRenderer sr;
    private float currentSpeed = 0f;
    private bool isDoingTask = false;
    private Coroutine moveRoutine;

    [Header("Waypoint")]
    public Waypoint startWaypoint;
    private Waypoint currentWaypoint;

    [Header("Thời gian chờ ban đầu")]
    public float waitTimeAtStart = 10f;

    [Header("Slider Final Seek")]
    public Slider finalSeekSlider; // gán slider thứ 2 vào đây

    private bool hasStartedMoving = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (startWaypoint != null)
        {
            currentWaypoint = startWaypoint;
            transform.position = currentWaypoint.transform.position;
        }

        // Bắt đầu trận → đứng yên
        currentSpeed = 0f;
        animator.SetBool("isRunning", false); // 🔹 Đảm bảo đứng yên animation

        StartCoroutine(StartDelayRoutine());
    }

    private IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(waitTimeAtStart);

        // Sau 10 giây → bắt đầu chạy
        currentSpeed = normalSeekSpeed;
        hasStartedMoving = true;
        moveRoutine = StartCoroutine(MoveToNextWaypoint()); // 🔹 Chỉ start di chuyển sau cooldown
    }

    private void Update()
    {
        // Nếu chưa bắt đầu di chuyển thì bỏ qua
        if (!hasStartedMoving) return;

        // Check slider final seek
        if (finalSeekSlider != null && finalSeekSlider.gameObject.activeSelf)
        {
            currentSpeed = finalSeekSpeed;
        }
        else
        {
            currentSpeed = normalSeekSpeed;
        }
    }

    private IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            if (currentWaypoint == null) yield break;

            Transform next = currentWaypoint.GetRandomNext();
            if (next == null) yield break;

            Vector3 target = next.position;
            animator.SetBool("isRunning", true); // 🔹 Chỉ bật khi bắt đầu move
            FlipDirection(target - transform.position);

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                if (isDoingTask)
                    yield return new WaitUntil(() => !isDoingTask);

                transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
                yield return null;
            }

            // Vì Impostor luôn chạy → không cần set isRunning = false ở đây
            currentWaypoint = next.GetComponent<Waypoint>();
        }
    }

    private void FlipDirection(Vector3 direction)
    {
        if (sr == null) return;
        if (direction.x > 0.01f) sr.flipX = false;
        else if (direction.x < -0.01f) sr.flipX = true;
    }
}
