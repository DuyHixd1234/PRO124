using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class VotingResultManager : MonoBehaviour
{
    [Header("Sprites cho AI (index 1–9)")]
    public Sprite[] indexToAISprite; // 9 sprites tương ứng Shuffle_Index_1...9

    [Header("Sprites cho Human dựa theo ColorIndex: 0=Red, 1=Yellow, 2=DarkGreen, 3=White")]
    public Sprite[] indexToHumanSprite; // 4 sprite tương ứng màu Human (index 0)

    [Header("Sprite riêng khi Skip")]
    public Sprite skipSprite;

    [Header("UI")]
    public Image ejectImage;
    public TMP_Text resultText;
    public GameObject animatedImageObject;
    public RectTransform animatedRectTransform;

    [Header("Effect Settings")]
    public float ejectDuration = 5f;
    public float rotateSpeed = 180f;
    public float startOffsetX = -1200f;
    public float endOffsetX = 1200f;

    [Header("Text Timing")]
    public float textStartDelay = 0f;
    public float textDuration = 3f;

    [Header("After Text Complete")]
    public GameObject afterTextObject;
    public float delayBeforeReturnToMap = 2f;

    [Header("New Settings - Instead of Load Scene")]
    public GameObject blackOverlay;   // Gán object đen (đang ẩn)
    public Canvas parentCanvas;       // Gán Canvas cha của panel này

    void Start()
    {
        string result = PlayerPrefs.GetString("EjectedResult", "None");

        if (result == "Tie")
        {
            StartCoroutine(PlayTextTyping("No one was ejected. (Tie)"));
            return;
        }

        if (result == "Skip")
        {
            ejectImage.sprite = skipSprite;
            StartCoroutine(PlayEjectAnimation());
            StartCoroutine(PlayTextTyping("No one was ejected. (Skipped)"));
            return;
        }

        if (result == "None")
        {
            StartCoroutine(PlayTextTyping("No data."));
            return;
        }

        int ejectedIndex = PlayerPrefs.GetInt("VotedOutIndex", -1);
        if (ejectedIndex < 0 || ejectedIndex > 9)
        {
            Debug.LogError("❌ VotedOutIndex không hợp lệ.");
            StartCoroutine(PlayTextTyping("Lỗi vote."));
            return;
        }

        Sprite selectedSprite = null;
        string nameEjected = PlayerPrefs.GetString($"Shuffle_Name_{ejectedIndex}", $"Unknown {ejectedIndex}");
        bool isImpostor = PlayerPrefs.GetInt($"Shuffle_Role_{ejectedIndex}", 0) == 1;

        if (ejectedIndex == 0)
        {
            int colorIndex = PlayerPrefs.GetInt("HumanColorIndex", 0);
            if (colorIndex >= 0 && colorIndex < indexToHumanSprite.Length)
                selectedSprite = indexToHumanSprite[colorIndex];
            else
                Debug.LogWarning($"⚠️ Không tìm thấy sprite Human color index {colorIndex}");
        }
        else
        {
            int aiSpriteIndex = ejectedIndex - 1;
            if (aiSpriteIndex >= 0 && aiSpriteIndex < indexToAISprite.Length)
                selectedSprite = indexToAISprite[aiSpriteIndex];
            else
                Debug.LogWarning($"⚠️ Không tìm thấy sprite AI cho index {ejectedIndex}");
        }

        if (selectedSprite != null)
        {
            ejectImage.sprite = selectedSprite;
        }
        else
        {
            ejectImage.enabled = false;
        }

        string finalText = $"{nameEjected} was{(isImpostor ? "" : " not")} An Impostor.";
        StartCoroutine(PlayEjectAnimation());
        StartCoroutine(PlayTextTyping(finalText));
    }

    IEnumerator PlayEjectAnimation()
    {
        animatedImageObject.SetActive(true);

        if (animatedRectTransform != null)
        {
            Vector2 startPos = new Vector2(startOffsetX, animatedRectTransform.anchoredPosition.y);
            Vector2 endPos = new Vector2(endOffsetX, animatedRectTransform.anchoredPosition.y);
            animatedRectTransform.anchoredPosition = startPos;

            float elapsed = 0f;
            while (elapsed < ejectDuration)
            {
                float t = elapsed / ejectDuration;
                animatedRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                animatedRectTransform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator PlayTextTyping(string finalText)
    {
        yield return new WaitForSeconds(textStartDelay);

        resultText.text = "";
        resultText.gameObject.SetActive(true);
        float charDelay = textDuration / Mathf.Max(1, finalText.Length);

        foreach (char c in finalText)
        {
            if (c != ' ' && SoundManager.Instance != null)
                SoundManager.Instance.PlayTypingSound();

            resultText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        if (afterTextObject != null)
            afterTextObject.SetActive(true);

        yield return new WaitForSeconds(delayBeforeReturnToMap);

        // Thay vì load scene, bật overlay đen và destroy canvas
        if (blackOverlay != null)
            blackOverlay.SetActive(true);

        if (parentCanvas != null)
            Destroy(parentCanvas.gameObject);
    }
}
