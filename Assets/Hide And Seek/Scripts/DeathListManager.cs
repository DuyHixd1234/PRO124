using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathListManager : MonoBehaviour
{
    [Header("Entities (0 = Player, 1–9 = AI)")]
    public GameObject[] entityList = new GameObject[10]; // 10 slot, index 0 là Player

    [Header("UI Settings")]
    public Image[] deathImages = new Image[9];    // 9 ảnh hiển thị (chỉ cho crewmate)
    public Sprite[] deadSprites = new Sprite[5];  // 5 sprite chết

    [Header("Transition Settings")]
    public GameObject beforeLoadObject; // Bật 1s trước khi load scene

    [Header("Impostor Flag (assign in Inspector)")]
    [Tooltip("Gán 1 GameObject mà bạn dùng làm flag Impostor. Mặc định OFF. Nếu ON => Player là Impostor.")]
    public GameObject impostorFlag;

    private bool[] wasActive;
    private int replacedDeathImages = 0;
    private bool playerIsImpostor = false;
    private int impostorIndex = -1;

    // tránh load scene nhiều lần
    private bool isSceneLoading = false;

    void Start()
    {
        // Xác định Impostor ngay khi bắt đầu (legacy logic hiện có)
        for (int i = 0; i < entityList.Length; i++)
        {
            // Nếu object bị missing (null) hoặc inactive ngay từ đầu → impostor (theo logic cũ của bạn)
            if (entityList[i] == null || !entityList[i].activeSelf)
            {
                impostorIndex = i;
                break;
            }
        }

        if (impostorIndex == -1)
        {
            Debug.LogError("Không tìm thấy Impostor! Tất cả entity đều đang active và không missing?");
        }
        else
        {
            Debug.Log($"[DeathListManager] Impostor index = {impostorIndex}");
        }

        // Xác định vai trò Player theo impostorIndex legacy
        playerIsImpostor = (impostorIndex == 0);

        // Lưu vào PlayerPrefs
        PlayerPrefs.SetInt("ImpostorIndex", impostorIndex);
        PlayerPrefs.SetInt("Player_IsImpostor", playerIsImpostor ? 1 : 0);
        PlayerPrefs.Save();

        // Khởi tạo mảng theo dõi trạng thái
        wasActive = new bool[entityList.Length];
        for (int i = 0; i < entityList.Length; i++)
        {
            wasActive[i] = (entityList[i] != null && entityList[i].activeSelf);
        }
    }

    void Update()
    {
        for (int i = 0; i < entityList.Length; i++)
        {
            GameObject entity = entityList[i];

            bool currentlyActive = (entity != null && entity.activeSelf);

            // Alive state
            if (currentlyActive)
            {
                PlayerPrefs.SetInt($"Alive_Index_{i}", 1);
            }

            // Death detection (từ active sang inactive hoặc bị destroy)
            if (wasActive[i] && !currentlyActive)
            {
                PlayerPrefs.SetInt($"Dead_Index_{i}", 1);
                HandleDeath(i);
            }

            wasActive[i] = currentlyActive;
        }

        // Kiểm tra kết thúc game (logic cũ)
        if (!isSceneLoading && replacedDeathImages >= deathImages.Length)
        {
            // Nếu mọi deathImages đã được thay (hầu như là tất cả crewmate đã bị chết)
            if (playerIsImpostor)
                StartCoroutine(LoadSceneWithDelay("HSWin"));
            else
                StartCoroutine(LoadSceneWithDelay("HSLose"));
        }
    }

    void HandleDeath(int entityIndex)
    {
        AddDeathSprite();
    }

    void AddDeathSprite()
    {
        for (int i = 0; i < deathImages.Length; i++)
        {
            Image targetImg = deathImages[i];
            if (targetImg != null && !IsDeadSprite(targetImg.sprite))
            {
                Sprite chosen = deadSprites[Random.Range(0, deadSprites.Length)];
                targetImg.sprite = chosen;
                replacedDeathImages++;
                break;
            }
        }
    }

    bool IsDeadSprite(Sprite sprite)
    {
        foreach (var dead in deadSprites)
        {
            if (sprite == dead) return true;
        }
        return false;
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        if (isSceneLoading) yield break;
        isSceneLoading = true;

        if (beforeLoadObject != null)
            beforeLoadObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    // -------------------------
    // NEW: gọi hàm này khi "thời gian hết"
    // -------------------------
    public void OnTimeExpired()
    {
        if (isSceneLoading) return; // tránh gọi nhiều lần

        // Nếu có impostorFlag và nó đang ON => Player là Impostor theo yêu cầu của bạn
        bool impostorFlagOn = (impostorFlag != null && impostorFlag.activeSelf);

        if (impostorFlagOn)
        {
            // Kiểm tra xem còn ít nhất 1 crewmate (index 1..n) đang active hay không
            bool anyCrewAlive = false;
            for (int i = 1; i < entityList.Length; i++)
            {
                if (entityList[i] != null && entityList[i].activeSelf)
                {
                    anyCrewAlive = true;
                    break;
                }
            }

            if (anyCrewAlive)
            {
                // Player là Impostor và vẫn còn 1 crewmate sống => Impostor thua
                StartCoroutine(LoadSceneWithDelay("HSLose"));
            }
            else
            {
                // Không còn crewmate => Impostor thắng
                StartCoroutine(LoadSceneWithDelay("HSWin"));
            }
        }
        else
        {
            // impostorFlag OFF hoặc không gán => mặc định crewmate thắng khi hết giờ
            StartCoroutine(LoadSceneWithDelay("HSWin"));
        }
    }
}
