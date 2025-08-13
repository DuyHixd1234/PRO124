using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMove : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 4f;
    public Animator animator;
    private SpriteRenderer sr;

    [Header("Waypoint")]
    public Waypoint startWaypoint;  // Gán ở Inspector
    private Waypoint currentWaypoint;

    private bool isDoingTask = false;
    private Coroutine moveRoutine;

    [Header("Canvas cần kiểm tra (ưu tiên dùng mảng này)")]
    public GameObject[] watchCanvases;  // Gán danh sách các canvas sẽ làm AI pause

    [Header("Tương thích cũ (tùy chọn)")]
    public GameObject canvasDeadBody;
    public GameObject canvasDiscuss;
    public GameObject canvasResult;

    // Nội bộ
    private readonly List<GameObject> _canvases = new List<GameObject>();
    private bool[] _prevCanvasStates;      // trạng thái frame trước của từng canvas
    private bool _pausedByCanvas = false;  // đang bị pause bởi canvas ON

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Gom tất cả canvas theo dõi vào 1 list, loại null, không trùng
        _canvases.Clear();
        if (watchCanvases != null)
        {
            for (int i = 0; i < watchCanvases.Length; i++)
            {
                var go = watchCanvases[i];
                if (go != null && !_canvases.Contains(go))
                    _canvases.Add(go);
            }
        }
        // Thêm 3 canvas cũ nếu có (để tương thích)
        if (canvasDeadBody != null && !_canvases.Contains(canvasDeadBody)) _canvases.Add(canvasDeadBody);
        if (canvasDiscuss != null && !_canvases.Contains(canvasDiscuss)) _canvases.Add(canvasDiscuss);
        if (canvasResult != null && !_canvases.Contains(canvasResult)) _canvases.Add(canvasResult);

        // Mảng trạng thái trước đó
        _prevCanvasStates = new bool[_canvases.Count];
        for (int i = 0; i < _prevCanvasStates.Length; i++)
            _prevCanvasStates[i] = IsActive(_canvases[i]);

        // Khởi tạo vị trí
        if (startWaypoint != null)
        {
            currentWaypoint = startWaypoint;
            transform.position = startWaypoint.transform.position;
            StartMoveRoutine();
        }
        else
        {
            Debug.LogWarning($"[AIMove] {gameObject.name} chưa gán startWaypoint.");
        }
    }

    void OnDisable()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    private void StartMoveRoutine()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            // Nếu đang pause bởi canvas ON -> đứng yên tại start
            if (_pausedByCanvas)
            {
                if (animator) animator.SetBool("isRunning", false);
                // Đảm bảo đứng đúng vị trí start
                if (startWaypoint != null)
                {
                    transform.position = startWaypoint.transform.position;
                }
                yield return null;
                continue;
            }

            if (currentWaypoint == null)
            {
                if (startWaypoint == null)
                {
                    Debug.LogWarning($"[AIMove] {gameObject.name} không có waypoint hiện tại và chưa gán startWaypoint.");
                    yield break;
                }
                currentWaypoint = startWaypoint;
            }

            Transform next = currentWaypoint.GetRandomNext();
            if (next == null)
            {
                if (animator) animator.SetBool("isRunning", false);
                Debug.LogWarning($"[AIMove] {gameObject.name} không có waypoint kế tiếp từ {currentWaypoint.name}.");
                yield break;
            }

            Vector3 target = next.position;
            if (animator) animator.SetBool("isRunning", true);
            FlipDirection(target - transform.position);

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                // Nếu đang làm task thì chờ
                if (isDoingTask)
                    yield return new WaitUntil(() => !isDoingTask);

                // Nếu trong quá trình chạy mà có canvas ON -> pause và kéo về start
                if (_pausedByCanvas)
                {
                    if (animator) animator.SetBool("isRunning", false);
                    break; // thoát khỏi vòng while, quay lên đầu loop để đứng tại start
                }

                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (animator) animator.SetBool("isRunning", false);

            // Nếu tạm thời bị pause (canvas vừa bật trong lúc chạy), quay lại đầu vòng lặp để đứng start
            if (_pausedByCanvas) continue;

            // Cập nhật waypoint hiện tại
            currentWaypoint = next.GetComponent<Waypoint>();
        }
    }

    void Update()
    {
        // Theo dõi chuyển trạng thái của các canvas
        bool anyActiveNow = false;
        bool anyActivatedThisFrame = false;

        for (int i = 0; i < _canvases.Count; i++)
        {
            bool activeNow = IsActive(_canvases[i]);
            anyActiveNow |= activeNow;

            // Edge detect: OFF -> ON
            if (activeNow && !_prevCanvasStates[i])
            {
                anyActivatedThisFrame = true;
            }

            _prevCanvasStates[i] = activeNow;
        }

        // Nếu có bất kỳ canvas vừa được bật -> pause & respawn về start
        if (anyActivatedThisFrame)
            PauseAndRespawnToStart();

        // Nếu không còn canvas nào active và đang bị pause -> resume từ start
        if (!anyActiveNow && _pausedByCanvas)
            ResumeFromStart();
    }

    private void PauseAndRespawnToStart()
    {
        _pausedByCanvas = true;

        // Dừng di chuyển ngay
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        // Teleport về start + reset hướng chạy
        if (startWaypoint != null)
        {
            transform.position = startWaypoint.transform.position;
            currentWaypoint = startWaypoint; // QUAN TRỌNG: từ nay chọn next từ start, không còn dùng waypoint cũ
        }

        if (animator) animator.SetBool("isRunning", false);
    }

    private void ResumeFromStart()
    {
        _pausedByCanvas = false;

        // Đảm bảo currentWaypoint là start trước khi chạy
        if (startWaypoint != null)
            currentWaypoint = startWaypoint;

        StartMoveRoutine();
    }

    private bool IsActive(GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public void StartTask(float duration)
    {
        if (isDoingTask) return;
        StartCoroutine(GoToTask(duration));
    }

    IEnumerator GoToTask(float duration)
    {
        isDoingTask = true;
        if (animator) animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(duration);
        isDoingTask = false;
    }

    private void FlipDirection(Vector3 direction)
    {
        if (sr == null) return;

        if (direction.x > 0.01f)
            sr.flipX = false;
        else if (direction.x < -0.01f)
            sr.flipX = true;
    }
}
