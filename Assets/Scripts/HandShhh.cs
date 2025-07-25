using UnityEngine;
using System.Collections;

public class HandShhh : MonoBehaviour
{
    public float moveSpeed = 60f;
    public float targetAngle = 0f;
    public float startDelay = 0.2f;

    public GameObject shhhImage;             // Hinh Shhhh!
    public Transform characterToShake;       // Con Among Us
    public GameObject canvasShhhh;           // Canvas goc chua tay va Shhhh

    private bool moving = false;
    private bool reached = false;

    public static bool shhhFinished = false; // Bien global

    void Start()
    {
        if (shhhImage != null)
            shhhImage.SetActive(false);

        Invoke(nameof(StartMoving), startDelay);
    }

    void StartMoving()
    {
        moving = true;
    }

    void Update()
    {
        if (!moving || reached) return;

        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        float step = moveSpeed * Time.deltaTime;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetAngle, step);
        transform.localEulerAngles = new Vector3(0, 0, newZ);

        if (Mathf.Abs(newZ - targetAngle) < 0.5f)
        {
            reached = true;
            if (shhhImage != null)
                shhhImage.SetActive(true);

            if (characterToShake != null)
                StartCoroutine(DoShakeEffect());

            StartCoroutine(DelayAndExitShhh());
        }
    }

    IEnumerator DoShakeEffect()
    {
        Vector3 originalScale = characterToShake.localScale;
        Vector3 enlarged = originalScale * 1.1f;

        characterToShake.localScale = enlarged;
        yield return new WaitForSeconds(0.1f);
        characterToShake.localScale = originalScale;
    }

    IEnumerator DelayAndExitShhh()
    {
        yield return new WaitForSeconds(1f); // Delay xem Shhhh

        if (canvasShhhh != null)
            canvasShhhh.SetActive(false);

        // Bao hieu cho Shuffle.cs bat UI
        shhhFinished = true;
    }
}
