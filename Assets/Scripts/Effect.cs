using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class Effect : MonoBehaviour
{
    [Header("Kích hoạt")]
    public bool playOnStart = true;
    public float delayBeforeActive = 0f;
    public float delayBeforeEffect = 0f;

    [Header("Fade Options")]
    public bool useFadeIn = false;
    public bool useFadeOut = false;
    public float fadeDuration = 1f;

    [Header("Slide Options")]
    public bool slideInFromLeft = false;
    public bool slideInFromRight = false;
    public bool slideInFromTop = false;
    public bool slideInFromBottom = false;

    public bool slideOutToLeft = false;
    public bool slideOutToRight = false;
    public bool slideOutToTop = false;
    public bool slideOutToBottom = false;

    public float slideDuration = 1f;

    [Header("Zoom In Options")]
    public bool useZoomIn = false;
    public float zoomDuration = 0.5f;
    public Vector3 zoomStartScale = new Vector3(3, 3, 1);

    [Header("Misc")]
    public bool resetOnDisable = true; // nếu true: khi object bị disable sẽ reset về trạng thái gốc

    // Internal
    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 initialAnchoredPos;
    private Vector3 initialScale;
    private float initialAlpha;
    private Vector2 slideStartPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Lưu trạng thái "gốc" chỉ 1 lần ở Awake
        initialAnchoredPos = rect.anchoredPosition;
        initialScale = transform.localScale;
        initialAlpha = canvasGroup.alpha;
    }

    void OnEnable()
    {
        // Reset về trạng thái gốc mỗi lần bật (để chạy hiệu ứng đúng vị trí ban đầu)
        StopAllCoroutines();
        rect.anchoredPosition = initialAnchoredPos;
        transform.localScale = initialScale;
        canvasGroup.alpha = initialAlpha;

        if (playOnStart)
            StartCoroutine(PlayEffect());
    }

    void OnDisable()
    {
        // Khi bị disable, đảm bảo reset lại (để lần sau bật lên đúng vị trí)
        if (resetOnDisable)
        {
            // Không StartCoroutine ở đây vì object đang bị tắt, chỉ cần set giá trị nội bộ
            rect.anchoredPosition = initialAnchoredPos;
            transform.localScale = initialScale;
            canvasGroup.alpha = initialAlpha;
            StopAllCoroutines();
        }
    }

    public void Play()
    {
        StopAllCoroutines();
        // Reset luôn trước khi chơi để tránh trạng thái còn dư từ lần trước
        rect.anchoredPosition = initialAnchoredPos;
        transform.localScale = initialScale;
        canvasGroup.alpha = initialAlpha;

        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        if (delayBeforeActive > 0f)
        {
            // Nếu object đang inactive, bật lại sau delay
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                yield return new WaitForSeconds(delayBeforeActive);
            }
            else
            {
                yield return new WaitForSeconds(delayBeforeActive);
            }
        }

        if (delayBeforeEffect > 0f)
            yield return new WaitForSeconds(delayBeforeEffect);

        // Thực hiện lần lượt (bạn có thể thay đổi thứ tự hoặc chạy song song nếu muốn)
        if (useFadeIn) yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        if (useFadeOut) yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        if (slideInFromLeft) yield return StartCoroutine(SlideFromDirection(Vector2.left, slideDuration));
        if (slideInFromRight) yield return StartCoroutine(SlideFromDirection(Vector2.right, slideDuration));
        if (slideInFromTop) yield return StartCoroutine(SlideFromDirection(Vector2.up, slideDuration));
        if (slideInFromBottom) yield return StartCoroutine(SlideFromDirection(Vector2.down, slideDuration));

        if (slideOutToLeft) yield return StartCoroutine(SlideToDirection(Vector2.left, slideDuration));
        if (slideOutToRight) yield return StartCoroutine(SlideToDirection(Vector2.right, slideDuration));
        if (slideOutToTop) yield return StartCoroutine(SlideToDirection(Vector2.up, slideDuration));
        if (slideOutToBottom) yield return StartCoroutine(SlideToDirection(Vector2.down, slideDuration));

        if (useZoomIn) yield return StartCoroutine(ZoomInEffect());
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        canvasGroup.alpha = from;
        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    IEnumerator SlideFromDirection(Vector2 dir, float duration)
    {
        // Tạo offset lớn dựa trên màn hình + kích thước rect (dùng đúng trục)
        Vector2 offset = new Vector2(
            dir.x * (Screen.width + rect.rect.width),
            dir.y * (Screen.height + rect.rect.height)
        );

        slideStartPos = initialAnchoredPos + offset;
        rect.anchoredPosition = slideStartPos;

        float timer = 0f;
        while (timer < duration)
        {
            rect.anchoredPosition = Vector2.Lerp(slideStartPos, initialAnchoredPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = initialAnchoredPos;
    }

    IEnumerator SlideToDirection(Vector2 dir, float duration)
    {
        Vector2 offset = new Vector2(
            dir.x * (Screen.width + rect.rect.width),
            dir.y * (Screen.height + rect.rect.height)
        );

        Vector2 targetPos = initialAnchoredPos + offset;
        float timer = 0f;
        Vector2 startPos = rect.anchoredPosition;

        while (timer < duration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = targetPos;

        // Tắt object sau khi slide out xong.
        // OnDisable sẽ reset anchoredPosition/scale/alpha về giá trị gốc
        gameObject.SetActive(false);
    }

    IEnumerator ZoomInEffect()
    {
        Vector3 startScale = zoomStartScale;
        Vector3 endScale = initialScale;

        float timer = 0f;
        while (timer < zoomDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / zoomDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
    }
}
