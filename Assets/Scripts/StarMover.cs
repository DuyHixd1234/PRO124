using UnityEngine;

public class StarMover : MonoBehaviour
{
    public float moveSpeed = 1f;

    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Out of views then destroy the star
        if (transform.position.x < Camera.main.transform.position.x - 10f)
        {
            Destroy(gameObject);
        }
    }
}
