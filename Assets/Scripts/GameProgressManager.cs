using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Tên AI theo thứ tự (0 → 8), khớp với Shuffle.cs")]
    [SerializeField] private List<string> aiNames;

    private HashSet<int> eliminatedAIIndices = new HashSet<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ khi load scene mới
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EliminateAI(int index)
    {
        if (!eliminatedAIIndices.Contains(index))
            eliminatedAIIndices.Add(index);
    }

    public bool IsAIEliminated(int index)
    {
        return eliminatedAIIndices.Contains(index);
    }

    public bool IsAIEliminated(string name)
    {
        int index = aiNames.IndexOf(name);
        return index >= 0 && IsAIEliminated(index);
    }
}
