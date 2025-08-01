using UnityEngine;
using System.Collections;

public class AIMove : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 4f;
    public Animator animator;
    private SpriteRenderer sr;

    [Header("Waypoint")]
    public Waypoint startWaypoint; // Gán ở Inspector
    private Waypoint currentWaypoint;

    private bool isDoingTask = false;
    private Coroutine moveRoutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (startWaypoint != null)
        {
            currentWaypoint = startWaypoint;
            transform.position = currentWaypoint.transform.position;
            moveRoutine = StartCoroutine(MoveToNextWaypoint());
        }
        else
        {
            Debug.LogWarning($"[AIMove] {gameObject.name} chưa gán startWaypoint.");
        }
    }

    IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            if (currentWaypoint == null)
            {
                Debug.LogWarning($"{gameObject.name} không có waypoint hiện tại.");
                yield break;
            }

            Transform next = currentWaypoint.GetRandomNext();

            if (next == null)
            {
                Debug.LogWarning($"{gameObject.name} không có waypoint kế tiếp.");
                yield break;
            }

            Vector3 target = next.position;
            animator.SetBool("isRunning", true);
            FlipDirection(target - transform.position);

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                if (isDoingTask) yield return new WaitUntil(() => !isDoingTask); // Dừng giữa đường nếu đang làm task
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            animator.SetBool("isRunning", false);
            currentWaypoint = next.GetComponent<Waypoint>();
            
        }
    }

    public void StartTask(float duration)
    {
        if (isDoingTask) return;
        StartCoroutine(GoToTask(duration));
    }

    IEnumerator GoToTask(float duration)
    {
        isDoingTask = true;

        animator.SetBool("isRunning", false); // Đứng yên làm task
        yield return new WaitForSeconds(duration); // Thời gian làm task

        isDoingTask = false;
    }

    private void FlipDirection(Vector3 direction)
    {
        if (direction.x > 0.01f)
            sr.flipX = false;
        else if (direction.x < -0.01f)
            sr.flipX = true;
    }
}
