using UnityEngine;

public class DisableElementsOnEnable : MonoBehaviour
{
    [Tooltip("Danh sách các GameObject sẽ bị tắt khi object này bật")]
    public GameObject[] elements;

    private void OnEnable()
    {
        if (elements != null)
        {
            foreach (var obj in elements)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}
