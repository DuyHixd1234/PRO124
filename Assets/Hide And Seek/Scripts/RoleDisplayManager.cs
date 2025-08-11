using UnityEngine;

public class RoleDisplayManager : MonoBehaviour
{
    [Header("Crew Objects (theo thứ tự màu)")]
    public GameObject crewRed;
    public GameObject crewYellow;
    public GameObject crewDarkGreen;
    public GameObject crewWhite;

    [Header("Impostor Objects (theo thứ tự màu)")]
    public GameObject impostorRed;
    public GameObject impostorYellow;
    public GameObject impostorDarkGreen;
    public GameObject impostorWhite;

    void Start()
    {
        // Tắt toàn bộ con trước
        DisableAll();

        // Lấy dữ liệu màu & vai trò từ PlayerData
        int colorIndex = PlayerData.Instance.selectedColorIndex; // 0 = Red, 1 = Yellow, 2 = DarkGreen, 3 = White
        bool isImpostor = PlayerData.Instance.isImpostor; // true = Impostor, false = Crew

        // Bật đúng đối tượng
        ShowRoleObject(colorIndex, isImpostor);
    }

    void DisableAll()
    {
        GameObject[] allObjects = {
            crewRed, crewYellow, crewDarkGreen, crewWhite,
            impostorRed, impostorYellow, impostorDarkGreen, impostorWhite
        };

        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    void ShowRoleObject(int colorIndex, bool isImpostor)
    {
        GameObject target = null;

        if (!isImpostor)
        {
            switch (colorIndex)
            {
                case 0: target = crewRed; break;
                case 1: target = crewYellow; break;
                case 2: target = crewDarkGreen; break;
                case 3: target = crewWhite; break;
            }
        }
        else
        {
            switch (colorIndex)
            {
                case 0: target = impostorRed; break;
                case 1: target = impostorYellow; break;
                case 2: target = impostorDarkGreen; break;
                case 3: target = impostorWhite; break;
            }
        }

        if (target != null)
            target.SetActive(true);
        else
            Debug.LogWarning("Không tìm thấy object phù hợp với màu & vai trò hiện tại!");
    }
}
