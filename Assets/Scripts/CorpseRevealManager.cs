using UnityEngine;

public class CorpseRevealManager : MonoBehaviour
{
    [Header("Gán nhân vật cần theo dõi (sẽ bị kill)")]
    public GameObject targetCharacter;

    [Header("Cái xác tương ứng (nằm dưới object này)")]
    public GameObject corpseObject;

    private bool hasSpawned = false;

    void Update()
    {
        if (!hasSpawned && targetCharacter != null && !targetCharacter.activeSelf)
        {
            SpawnCorpseAtTarget();
            hasSpawned = true;
        }
    }

    void SpawnCorpseAtTarget()
    {
        // Đưa cái xác đến vị trí target
        corpseObject.transform.position = targetCharacter.transform.position;

        // Hiển thị xác
        corpseObject.SetActive(true);
    }
}
