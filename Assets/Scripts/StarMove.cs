using UnityEngine;

[DisallowMultipleComponent]
public class StarMove : MonoBehaviour
{
    // cấu hình "cứng" trong code (không expose ra Inspector)
    private const float MIN_SPEED = 10f;
    private const float MAX_SPEED = 80f;
    private const float OUT_OF_SCREEN_X = 5000f;
    private const float SPEED_MULTIPLIER = 2f;

    // hướng cố định (giữ nguyên)
    private static readonly Vector2 DIRECTION = Vector2.right;

    // cache
    private RectTransform rectTransform;
    private float moveSpeed;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("[StarMove] RectTransform not found on " + gameObject.name);
            enabled = false;
            return;
        }

        // random tốc độ 1 lần duy nhất cho mỗi object
        moveSpeed = Random.Range(MIN_SPEED, MAX_SPEED);
    }

    void Update()
    {
        // safety
        if (rectTransform == null) return;

        // di chuyển (tính toán bằng local var để giảm truy cập thành phần)
        float delta = moveSpeed * SPEED_MULTIPLIER * Time.deltaTime;
        Vector2 pos = rectTransform.anchoredPosition;
        pos += DIRECTION * delta;
        rectTransform.anchoredPosition = pos;

        // kiểm tra điểm kết thúc
        if (pos.x > OUT_OF_SCREEN_X)
        {
            Destroy(gameObject);
        }
    }
}
