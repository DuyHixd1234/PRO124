using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect : MonoBehaviour
{
    [Header("Kich hoat")]
    public bool playOnStart = true;           // Tu dong chay khi Start
    [Tooltip("Sau bao giay thi object duoc bat len (neu dang bi tat)")]
    public float delayBeforeActive = 0f;

    [Tooltip("Sau bao giay tu luc object bat len thi moi chay hieu ung")]
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
    private Vector3 originalPos;

    [Header("Zoom In Options")]
    public bool useZoomIn = false;
    public float zoomDuration = 0.5f;
    public Vector3 zoomStartScale = new Vector3(3, 3, 1);

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector3 slideStartPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        originalPos = rect.anchoredPosition;
    }

    void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayEffect());
    }

    public void Play()
    {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        if (delayBeforeActive > 0f)
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(delayBeforeActive);
            gameObject.SetActive(true);
        }

        if (delayBeforeEffect > 0f)
            yield return new WaitForSeconds(delayBeforeEffect);

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
        // Slide In: KHÔNG tắt gameObject
        slideStartPos = originalPos + new Vector3(dir.x, dir.y, 0) * Screen.width;
        rect.anchoredPosition = slideStartPos;

        float timer = 0f;
        while (timer < duration)
        {
            rect.anchoredPosition = Vector3.Lerp(slideStartPos, originalPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = originalPos;
    }

    IEnumerator SlideToDirection(Vector2 dir, float duration)
    {
        // Slide Out: TẮT object SAU KHI hiệu ứng kết thúc
        Vector3 targetPos = originalPos + new Vector3(dir.x, dir.y, 0) * Screen.width;

        float timer = 0f;
        while (timer < duration)
        {
            rect.anchoredPosition = Vector3.Lerp(originalPos, targetPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = targetPos;

        // Sau khi hoàn tất mới tắt object
        gameObject.SetActive(false);
    }


    IEnumerator ZoomInEffect()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = zoomStartScale;

        float timer = 0f;
        while (timer < zoomDuration)
        {
            transform.localScale = Vector3.Lerp(zoomStartScale, originalScale, timer / zoomDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
