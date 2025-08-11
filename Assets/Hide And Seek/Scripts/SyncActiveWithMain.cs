using UnityEngine;

public class SyncActiveWithMain : MonoBehaviour
{
    [Header("Main Object")]
    public GameObject mainObject;

    [Header("Elements Objects")]
    public GameObject[] elementsObjects;

    void Update()
    {
        if (mainObject == null || elementsObjects == null) return;

        // Nếu mainObject inactive thì set tất cả elements inactive
        if (!mainObject.activeSelf)
        {
            foreach (var obj in elementsObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
