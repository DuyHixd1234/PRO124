using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    [Header("Xác (GameObjects con)")]
    public GameObject[] corpses = new GameObject[9];

    [Header("Canvas liên kết với mỗi xác")]
    public Canvas[] linkedCanvases = new Canvas[9];

    [Header("Cấu hình")]
    [Tooltip("Thời gian (giây) trong đó sẽ kiểm tra và Destroy các corpse đang active")]
    public float windowDuration = 1f;

    [Tooltip("Bật log debug")]
    public bool debugLogs = false;

    // Internal state để phát hiện transition exist -> destroyed (non-null -> null)
    private bool[] prevCanvasExists;
    private bool isWindowActive = false;

    void Awake()
    {
        // Khởi tạo prev array theo kích thước linkedCanvases
        int n = linkedCanvases != null ? linkedCanvases.Length : 0;
        prevCanvasExists = new bool[n];

        for (int i = 0; i < n; i++)
        {
            prevCanvasExists[i] = (linkedCanvases[i] != null);
        }
    }

    void Update()
    {
        // Nếu đang trong cửa sổ xử lý thì không phát hiện event mới
        if (isWindowActive) return;

        if (linkedCanvases == null || prevCanvasExists == null) return;

        int len = Mathf.Min(prevCanvasExists.Length, linkedCanvases.Length);

        for (int i = 0; i < len; i++)
        {
            bool currentlyExists = linkedCanvases[i] != null;

            // Nếu trước đó không tồn tại nhưng giờ reappeared => cập nhật prev = true
            if (!prevCanvasExists[i] && currentlyExists)
            {
                prevCanvasExists[i] = true;
                if (debugLogs) Debug.Log($"[CorpseManager] Canvas reappeared at index {i}");
                continue;
            }

            // Detect transition existed -> destroyed (true -> null)
            if (prevCanvasExists[i] && !currentlyExists)
            {
                if (debugLogs) Debug.Log($"[CorpseManager] Detected Canvas DESTROY at index {i}. Starting {windowDuration}s destroy window.");
                prevCanvasExists[i] = false; // consume event cho index này
                StartCoroutine(HandleDestroyWindow(windowDuration));
                break; // thoát for: chỉ cần 1 canvas bị destroy là kích hoạt hành động
            }
        }
    }

    private IEnumerator HandleDestroyWindow(float duration)
    {
        isWindowActive = true;
        float elapsed = 0f;

        // Dùng HashSet để tránh destroy trùng lặp cùng object trong window (không thực sự cần nhưng an toàn)
        HashSet<GameObject> destroyedThisWindow = new HashSet<GameObject>();

        // Lặp cho tới duration: mỗi frame kiểm tra và destroy các corpses đang active
        while (elapsed < duration)
        {
            for (int i = 0; i < corpses.Length; i++)
            {
                var c = corpses[i];
                if (c == null) continue;
                if (destroyedThisWindow.Contains(c)) continue;

                if (c.activeSelf)
                {
                    if (debugLogs) Debug.Log($"[CorpseManager] Destroying corpse index {i}: {c.name}");
                    Destroy(c);
                    destroyedThisWindow.Add(c);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (debugLogs) Debug.Log("[CorpseManager] Destroy window ended.");
        isWindowActive = false;
    }

    // Hỗ trợ tool: gọi thủ công để force rebuild prevCanvasExists array (ví dụ khi bạn gán canvas mới vào inspector runtime)
    public void RefreshCanvasState()
    {
        if (linkedCanvases == null) return;
        if (prevCanvasExists == null || prevCanvasExists.Length != linkedCanvases.Length)
            prevCanvasExists = new bool[linkedCanvases.Length];

        for (int i = 0; i < linkedCanvases.Length; i++)
            prevCanvasExists[i] = (linkedCanvases[i] != null);

        if (debugLogs) Debug.Log("[CorpseManager] Refreshed canvas state.");
    }
}
