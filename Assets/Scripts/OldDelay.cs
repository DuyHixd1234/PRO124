using UnityEngine;

public class OldDelay : MonoBehaviour
{
    [Header("Target object to activate")]
    public GameObject targetObject;

    [Header("Delay (seconds)")]
    public float delay = 1f;

    void Start()
    {
        if (targetObject != null)
        {
            StartCoroutine(ActivateAfterDelay());
        }
    }

    System.Collections.IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(true);
    }
}