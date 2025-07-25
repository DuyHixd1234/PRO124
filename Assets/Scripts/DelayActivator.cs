using UnityEngine;

public class DelayActivator : MonoBehaviour
{
    public GameObject targetObject;
    public float delay = 1f;

    void Start()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    System.Collections.IEnumerator ActivateAfterDelay()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            yield return new WaitForSeconds(delay);
            targetObject.SetActive(true);

            // THÊM DÒNG NÀY
            Effect fx = targetObject.GetComponent<Effect>();
            if (fx != null) fx.Play();
        }
    }
}
