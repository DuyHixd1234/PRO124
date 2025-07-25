using UnityEngine;

public class ImpostorWiggle : MonoBehaviour
{
    public float dropDistance = 40f;      // Khoảng cách rơi từ trên xuống (so với vị trí ban đầu)
    public float dropSpeed = 3.5f;           // Tốc độ rơi xuống
    private Vector3 targetPosition;
    private bool isDropping = true;

    void Start()
    {
        targetPosition = transform.localPosition;
        transform.localPosition = targetPosition + Vector3.up * dropDistance;
    }

    void Update()
    {
        if (isDropping)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, dropSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                transform.localPosition = targetPosition;
                isDropping = false;
            }
        }
    }
}
