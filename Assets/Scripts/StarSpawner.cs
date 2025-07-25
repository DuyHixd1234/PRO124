using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject[] starPrefabs;     
    public float spawnInterval = 0.1f;    
    public float minY = -5f, maxY = 5f;   
    public float minScale = 0.2f, maxScale = 1.0f; 
    public float minSpeed = 1f, maxSpeed = 4f;    

    void Start()
    {
        InvokeRepeating("SpawnStar", 0f, spawnInterval);
    }

    void SpawnStar()
    {
       
        int index = Random.Range(0, starPrefabs.Length);
        GameObject star = Instantiate(starPrefabs[index]);

       
        float y = Random.Range(minY, maxY);
        star.transform.position = new Vector3(Camera.main.transform.position.x + 10f, y, 0f);

        float scale = Random.Range(minScale, maxScale);
        star.transform.localScale = new Vector3(scale, scale, 1f);

        float speed = Random.Range(minSpeed, maxSpeed);
        star.AddComponent<StarMover>().moveSpeed = speed;
    }
}
