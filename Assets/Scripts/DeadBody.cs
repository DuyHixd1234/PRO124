using UnityEngine;

public class DeadBody : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Body";
            sr.sortingOrder = 10;
        }
    }
}
