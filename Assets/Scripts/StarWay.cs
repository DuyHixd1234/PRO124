using UnityEngine;

public class StarWay : MonoBehaviour
{
    public float moveSpeed = 1f;

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < Camera.main.transform.position.y - 10f)
        {
            Destroy(gameObject);
        }
    }
}
