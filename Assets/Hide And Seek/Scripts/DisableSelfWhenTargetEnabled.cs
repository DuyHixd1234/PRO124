using UnityEngine;

public class DisableSelfWhenTargetEnabled : MonoBehaviour
{
    [Tooltip("Object cần theo dõi trạng thái bật/tắt")]
    public GameObject targetObject;

    private void Update()
    {
        if (targetObject != null && targetObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
