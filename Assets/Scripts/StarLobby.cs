using UnityEngine;

public class StarLobby : MonoBehaviour
{
    public GameObject[] starPrefabs;
    public float spawnInterval = 0.1f;
    public float minScale = 0.2f, maxScale = 1.0f;
    public float minSpeed = 1f, maxSpeed = 4f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        InvokeRepeating("SpawnStar", 0f, spawnInterval);
    }

    void SpawnStar()
    {
        if (starPrefabs.Length == 0) return;

        int index = Random.Range(0, starPrefabs.Length);
        GameObject star = Instantiate(starPrefabs[index]);

        // Tính minX, maxX theo viewport để đảm bảo nằm trong camera
        float camY = cam.transform.position.y;
        float camZ = cam.transform.position.z;
        float spawnY = camY + cam.orthographicSize + 1f; // +1 để spawn ngay trên rìa trên camera

        // World position theo viewport (0 là trái, 1 là phải)
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0f, -camZ));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0f, -camZ));

        float minX = left.x;
        float maxX = right.x;

        float x = Random.Range(minX, maxX);
        star.transform.position = new Vector3(x, spawnY, 0f);

        float scale = Random.Range(minScale, maxScale);
        star.transform.localScale = new Vector3(scale, scale, 1f);

        float speed = Random.Range(minSpeed, maxSpeed);
        star.AddComponent<StarWay>().moveSpeed = speed;
    }
}
