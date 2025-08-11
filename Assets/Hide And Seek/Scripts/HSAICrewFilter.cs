using UnityEngine;

public class HSAICrewFilter : MonoBehaviour
{
    [Header("Danh sách 9 AI Crewmates (Elements)")]
    public GameObject[] aiCrewmates = new GameObject[9]; // index 0 -> AI1, index 8 -> AI9

    private void Start()
    {
        // Đảm bảo mảng đủ 9 phần tử
        if (aiCrewmates.Length != 9)
        {
            Debug.LogError("[HSAICrewFilter] Cần gán đúng 9 AI trong Inspector!");
            return;
        }

        // Kiểm tra từng AI từ index 1-9 trong HSShuffle
        for (int i = 1; i <= 9; i++) // Bỏ index 0 vì đó là Human
        {
            int role = PlayerPrefs.GetInt($"Shuffle_Role_{i}", 0); // 1 = Impostor, 0 = Crewmate

            if (role == 1) // Là Impostor
            {
                if (aiCrewmates[i - 1] != null)
                {
                    Destroy(aiCrewmates[i - 1]); // Xóa hẳn GameObject
                }
            }
            else
            {
                if (aiCrewmates[i - 1] != null)
                {
                    aiCrewmates[i - 1].SetActive(true);
                }
            }
        }
    }
}
