using UnityEngine;
using System.Collections;

public class DoorSlide : MonoBehaviour
{
    public RectTransform doorLeft;
    public RectTransform doorRight;
    public float slideDistance = 1500f;
    public float slideDuration = 1f;

    public void StartSlide()
    {
        StartCoroutine(SlideDoors());
    }

    IEnumerator SlideDoors()
    {
        Vector2 leftStart = doorLeft.anchoredPosition;
        Vector2 rightStart = doorRight.anchoredPosition;

        Vector2 leftTarget = leftStart + Vector2.left * slideDistance;
        Vector2 rightTarget = rightStart + Vector2.right * slideDistance;

        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            doorLeft.anchoredPosition = Vector2.Lerp(leftStart, leftTarget, t);
            doorRight.anchoredPosition = Vector2.Lerp(rightStart, rightTarget, t);
            yield return null;
        }

        // Optional: Hide or destroy doors
        doorLeft.gameObject.SetActive(false);
        doorRight.gameObject.SetActive(false);
    }
}
