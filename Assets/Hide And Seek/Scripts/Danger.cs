using UnityEngine;
using UnityEngine.UI;

public class Danger : MonoBehaviour
{
    [Header("Vòng tròn Danger (GameObject, có collider 2D trigger)")]
    public GameObject[] dangerCircles = new GameObject[5]; // 5 vòng tròn

    [Header("UI Images Danger Level 0-5")]
    public Image[] dangerImages = new Image[6]; // index = level

    [Header("GameObject chứa âm thanh loop (đã play sẵn)")]
    public GameObject audioSafeObj;  // danger 0
    public GameObject audioLowObj;   // danger 1-2
    public GameObject audioMidObj;   // danger 3-4
    public GameObject audioHighObj;  // danger 5

    private AudioSource audioSafe;
    private AudioSource audioLow;
    private AudioSource audioMid;
    private AudioSource audioHigh;

    private bool[] circleDetected;
    private int currentDanger = 0;

    void Awake()
    {
        circleDetected = new bool[dangerCircles.Length];
    }

    void Start()
    {
        Debug.Log("[Danger] Setup DangerZone và lấy AudioSource từ GameObject gán sẵn...");

        // Gắn script DangerZone2D vào từng vòng tròn
        for (int i = 0; i < dangerCircles.Length; i++)
        {
            if (dangerCircles[i] == null)
            {
                Debug.LogWarning($"[Danger] Vòng tròn index {i} chưa gán!");
                continue;
            }

            DangerZone2D dz = dangerCircles[i].GetComponent<DangerZone2D>();
            if (dz == null) dz = dangerCircles[i].AddComponent<DangerZone2D>();

            dz.zoneIndex = i;
            dz.parentDanger = this;

            Collider2D col = dangerCircles[i].GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        // Lấy AudioSource từ các GameObject gán sẵn
        audioSafe = GetAudioFromObject(audioSafeObj, "Safe");
        audioLow  = GetAudioFromObject(audioLowObj, "Low");
        audioMid  = GetAudioFromObject(audioMidObj, "Mid");
        audioHigh = GetAudioFromObject(audioHighObj, "High");

        UpdateDangerUI();
        UpdateDangerAudio();
    }

    void Update()
    {
        int totalDetected = 0;
        for (int i = 0; i < circleDetected.Length; i++)
        {
            if (circleDetected[i]) totalDetected++;
        }

        if (totalDetected != currentDanger)
        {
            Debug.Log($"[Danger] Level thay đổi {currentDanger} → {totalDetected}");
            currentDanger = totalDetected;
            UpdateDangerUI();
            UpdateDangerAudio();
        }
    }

    private void UpdateDangerUI()
    {
        for (int i = 0; i < dangerImages.Length; i++)
        {
            if (dangerImages[i] != null)
                dangerImages[i].gameObject.SetActive(i == currentDanger);
        }
    }

    private void UpdateDangerAudio()
    {
        Debug.Log($"[Danger] UpdateDangerAudio - Level {currentDanger}");

        SetAllVolume(0f);

        if (currentDanger == 0 && audioSafe != null) audioSafe.volume = 1f;
        else if (currentDanger <= 2 && audioLow != null) audioLow.volume = 1f;
        else if (currentDanger <= 4 && audioMid != null) audioMid.volume = 1f;
        else if (audioHigh != null) audioHigh.volume = 1f;
    }

    private AudioSource GetAudioFromObject(GameObject obj, string label)
    {
        if (obj == null)
        {
            Debug.LogWarning($"[Danger] GameObject âm thanh '{label}' chưa được gán!");
            return null;
        }

        AudioSource src = obj.GetComponent<AudioSource>();
        if (src == null)
        {
            Debug.LogWarning($"[Danger] GameObject '{label}' không có AudioSource!");
            return null;
        }

        if (!src.isPlaying)
        {
            src.loop = true;
            src.Play();
            Debug.Log($"[Danger] Auto-play audio '{label}'");
        }

        src.volume = 0f; // ban đầu tắt tiếng
        return src;
    }

    private void SetAllVolume(float value)
    {
        if (audioSafe != null) audioSafe.volume = value;
        if (audioLow != null) audioLow.volume = value;
        if (audioMid != null) audioMid.volume = value;
        if (audioHigh != null) audioHigh.volume = value;
    }

    // Nhận tín hiệu từ DangerZone2D
    public void SetCircleDetected(int index, bool detected)
    {
        circleDetected[index] = detected;
        Debug.Log($"[Danger] Vòng {index + 1} {(detected ? "PHÁT HIỆN" : "MẤT")}");
    }

    // Khi bị disable (hoặc setActive(false))
    private void OnDisable()
    {
        Debug.Log("[Danger] Bị disable → Reset về Level 0 và tắt âm thanh");
        currentDanger = 0;
        SetAllVolume(0f);
        UpdateDangerUI();
    }
}
