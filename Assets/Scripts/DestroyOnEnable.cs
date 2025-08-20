using UnityEngine;

public class DestroyOnEnable : MonoBehaviour
{
    [Header("Các GameObject sẽ bị Destroy khi bật object này")]
    public GameObject[] elements;

    private void OnEnable()
    {
        if (elements == null || elements.Length == 0) return;

        foreach (GameObject obj in elements)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
}
