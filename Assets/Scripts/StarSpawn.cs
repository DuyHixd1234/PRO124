using UnityEngine;
using UnityEngine.UI;

public class StarSpawn : MonoBehaviour
{
    [Header("Prefab Image co script StarMove (phai deactive trong prefab)")]
    public GameObject imageTemplate;

    [Header("Transform cha (Canvas UI)")]
    public Transform parentTransform;

    [Header("Spawn settings")]
    public float spawnInterval = 0.2f;
    public float startX = -1000f;
    public float minY = -300f;
    public float maxY = 300f;
    public float minSpeed = 20f;
    public float maxSpeed = 100f;
    public float minScale = 0.3f;
    public float maxScale = 1.0f;

    private float timer;

    void Start()
    {
        if (imageTemplate == null || parentTransform == null)
        {
            Debug.LogError("Thieu imageTemplate hoac parentTransform!");
            enabled = false;
            return;
        }

        imageTemplate.SetActive(false); // phòng trường hợp prefab để active
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnStar();
            timer = spawnInterval;
        }
    }

    void SpawnStar()
    {
        GameObject newStar = Instantiate(imageTemplate, parentTransform);
        newStar.SetActive(true);

        RectTransform rectTransform = newStar.GetComponent<RectTransform>();
        float randomY = Random.Range(minY, maxY);
        float randomScale = Random.Range(minScale, maxScale);
        float randomSpeed = Random.Range(minSpeed, maxSpeed);

        rectTransform.anchoredPosition = new Vector2(startX, randomY);
        rectTransform.localScale = Vector3.one * randomScale;

        StarMove starMove = newStar.GetComponent<StarMove>();
        if (starMove != null)
        {
            starMove.moveSpeed = randomSpeed;
        }
    }
}
