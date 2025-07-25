using UnityEngine;

public class AIEliminationManual : MonoBehaviour
{
    [Header("Thứ tự 9 AI cố định (AI_0 → AI_8)")]
    public GameObject[] AIs;

    void Start()
    {
        for (int i = 0; i < AIs.Length; i++)
        {
            if (GameProgressManager.Instance != null &&
                GameProgressManager.Instance.IsAIEliminated(i))
            {
                if (AIs[i] != null) AIs[i].SetActive(false);
            }
        }
    }
}
