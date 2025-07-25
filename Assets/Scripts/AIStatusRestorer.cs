using UnityEngine;

public class AIStatusRestorer : MonoBehaviour
{
    public string aiName;

    void Start()
    {
        if (GameProgressManager.Instance != null &&
            GameProgressManager.Instance.IsAIEliminated(aiName))
        {
            gameObject.SetActive(false);
        }
    }
}
