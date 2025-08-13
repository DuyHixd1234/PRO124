using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeathDisplayManager : MonoBehaviour
{
    [Header("Gán 9 AI theo màu (theo đúng thứ tự)")]
    public GameObject Blue;
    public GameObject Cyan;
    public GameObject Coral;
    public GameObject Brown;
    public GameObject Purple;
    public GameObject Gray;
    public GameObject Pink;
    public GameObject Lime;
    public GameObject Orange;

    [Header("Gán 9 Sprite tương ứng (cùng thứ tự với AI trên)")]
    public Sprite BlueSprite;
    public Sprite CyanSprite;
    public Sprite CoralSprite;
    public Sprite BrownSprite;
    public Sprite PurpleSprite;
    public Sprite GraySprite;
    public Sprite PinkSprite;
    public Sprite LimeSprite;
    public Sprite OrangeSprite;

    [Header("Odd Images (lẻ) - OddImages[0] là center")]
    public Image[] OddImages; // size 9

    [Header("Even Images (chẵn)")]
    public Image[] EvenImages; // size 8

    [Header("Panel chứa các image hiển thị")]
    public GameObject DeathDisplayPanel;

    [Header("Bản sao tiếp theo (phase kế tiếp)")]
    public GameObject nextPhaseObject;

    [Header("Canvas Deadbody của từng AI (9 cái, disable sẵn)")]
    public GameObject[] aiDeathCanvases; // size 9

    [Header("Canvas Discuss")]
    public GameObject discussCanvas;

    // ----------------- Nội bộ -----------------
    private GameObject[] aiObjs;   // mảng 9 AI đã chuẩn hóa
    private Sprite[] aiSprites;    // mảng 9 sprite đã chuẩn hóa

    // Indices theo dõi trong phase hiện tại
    private HashSet<int> aliveIndices = new HashSet<int>();     // đang on tại đầu phase
    private HashSet<int> removedIndices = new HashSet<int>();   // null/missing hoặc đã Destroy vì off ngay đầu phase
    private HashSet<int> killsThisPhase = new HashSet<int>();   // on->off trong phase

    // Dùng để detect on->off trong phase (không dùng global)
    private Dictionary<int, bool> prevAlive = new Dictionary<int, bool>();

    // Danh sách sprite sẽ hiển thị (chỉ build khi discuss bật)
    private List<Sprite> displaySprites = new List<Sprite>();

    private bool hasShownThisDiscuss = false;          // đã đổ ảnh trong discuss của phase này chưa
    private bool waitingAdvance = false;               // đang đợi chuyển phase
    private bool lastDiscussActive = false;            // track trạng thái discuss

    // -------------------------------------------------
    // Phase init: dùng OnEnable vì mỗi object (phase) bật lên một lần
    // -------------------------------------------------
    void OnEnable()
    {
        // Chuẩn hóa mảng từ public fields (tránh dict key là GameObject bị destroy)
        aiObjs = new GameObject[] { Blue, Cyan, Coral, Brown, Purple, Gray, Pink, Lime, Orange };
        aiSprites = new Sprite[] { BlueSprite, CyanSprite, CoralSprite, BrownSprite, PurpleSprite, GraySprite, PinkSprite, LimeSprite, OrangeSprite };

        // Reset toàn bộ dữ liệu phase
        aliveIndices.Clear();
        removedIndices.Clear();
        killsThisPhase.Clear();
        prevAlive.Clear();
        displaySprites.Clear();
        hasShownThisDiscuss = false;
        waitingAdvance = false;
        lastDiscussActive = (discussCanvas != null && discussCanvas.activeSelf);

        // Dọn dẹp: slot null/missing → bỏ qua; slot off ngay đầu phase → Destroy luôn & loại khỏi xử lý
        for (int i = 0; i < aiObjs.Length; i++)
        {
            var obj = aiObjs[i];

            if (obj == null)
            {
                removedIndices.Add(i); // missing: không đụng, cũng không hiển thị
                aiObjs[i] = null;
                continue;
            }

            // safe: activeSelf chỉ đọc khi obj != null
            if (!obj.activeSelf)
            {
                // off ngay đầu phase -> Destroy và loại hẳn slot
                // (đây là các kill của phase trước, không hiển thị ở phase này)
                Destroy(obj);
                aiObjs[i] = null;
                removedIndices.Add(i);
                continue;
            }

            // còn sống (on) tại đầu phase -> bắt đầu theo dõi
            aliveIndices.Add(i);
            prevAlive[i] = true; // đang on
        }

        // Ẩn toàn bộ image khi bắt đầu phase
        HideAllImages();
    }

    void Update()
    {
        // 1) Theo dõi kill mới trong phase này (on->off)
        TrackPhaseKills();

        // 2) Khi discuss vừa bật -> build danh sách hiển thị và đổ ảnh
        bool discussActive = (discussCanvas != null && discussCanvas.activeSelf);
        if (discussActive && !lastDiscussActive)
        {
            // discuss vừa transition từ off -> on
            BuildDisplaySpritesFromPhaseKills();
            ShowIfPanelReady(); // chỉ đổ ảnh khi panel cũng đang bật
        }
        lastDiscussActive = discussActive;

        // 3) Nếu Panel bật (và discuss đang bật) mà chưa show -> show
        ShowIfPanelReady();

        // 4) Khi discuss tắt -> chờ 5s rồi bật phase tiếp theo (sau khi xóa dữ liệu)
        if (!waitingAdvance && !discussActive && hasShownThisDiscuss)
        {
            StartCoroutine(AdvanceAfterDiscussOff());
            waitingAdvance = true;
        }

        // 5) Kiểm tra canvas deadbody bật để bật discuss (logic cũ, giữ lại)
        CheckAIcanvasActivation();
    }

    // Phát hiện kill trong phase: chỉ coi là kill khi obj từ on -> off trong khi phase này đang chạy
    private void TrackPhaseKills()
    {
        for (int i = 0; i < aiObjs.Length; i++)
        {
            if (removedIndices.Contains(i)) continue; // slot đã loại bỏ
            var obj = aiObjs[i];
            if (obj == null) continue; // missing/destroyed (không phải kill mới trong phase)

            bool wasAlive = prevAlive.ContainsKey(i) ? prevAlive[i] : true;
            bool isAlive = obj.activeSelf;

            if (wasAlive && !isAlive)
            {
                // Kill xảy ra trong phase hiện tại
                if (!killsThisPhase.Contains(i))
                    killsThisPhase.Add(i);

                prevAlive[i] = false;

                // KHÔNG destroy ở đây — để phase sau xử lý destroy theo yêu cầu
            }
            else
            {
                prevAlive[i] = isAlive;
            }
        }
    }

    // Build displaySprites chỉ từ các kill của phase hiện tại, bỏ qua mọi slot đã loại
    private void BuildDisplaySpritesFromPhaseKills()
    {
        displaySprites.Clear();

        foreach (int idx in killsThisPhase)
        {
            // Slot đã bị loại (off ngay đầu phase / missing) thì ignore
            if (removedIndices.Contains(idx)) continue;

            // Nếu object đã được set off bởi kill ở phase này, vẫn còn tồn tại (chưa destroy) -> lấy sprite
            Sprite sp = (idx >= 0 && idx < aiSprites.Length) ? aiSprites[idx] : null;
            if (sp != null)
                displaySprites.Add(sp);
        }
    }

    // Chỉ show khi DeathDisplayPanel đang bật VÀ discuss đang bật, và chưa show lần nào trong discuss này
    private void ShowIfPanelReady()
    {
        if (hasShownThisDiscuss) return;

        bool panelActive = (DeathDisplayPanel != null && DeathDisplayPanel.activeSelf);
        bool discussActive = (discussCanvas != null && discussCanvas.activeSelf);

        if (panelActive && discussActive)
        {
            DisplaySprites(displaySprites);
            hasShownThisDiscuss = true;
        }
    }

    // Đổ sprite theo lẻ/chẵn, hoàn toàn null-safe
    private void DisplaySprites(List<Sprite> sprites)
    {
        HideAllImages();

        if (sprites == null || sprites.Count == 0)
        {
            Debug.Log($"[DeathDisplay] {gameObject.name} - Không có kill trong phase này để hiển thị.");
            return;
        }

        int count = sprites.Count;

        if (count % 2 == 1) // lẻ
        {
            if (OddImages != null && OddImages.Length > 0 && OddImages[0] != null)
            {
                OddImages[0].sprite = sprites[0];
                OddImages[0].gameObject.SetActive(true);
            }

            for (int i = 1; i < count && i < (OddImages != null ? OddImages.Length : 0); i++)
            {
                var img = OddImages[i];
                if (img == null) continue;
                img.sprite = sprites[i];
                img.gameObject.SetActive(true);
            }
        }
        else // chẵn
        {
            for (int i = 0; i < count && i < (EvenImages != null ? EvenImages.Length : 0); i++)
            {
                var img = EvenImages[i];
                if (img == null) continue;
                img.sprite = sprites[i];
                img.gameObject.SetActive(true);
            }
        }
    }

    private void HideAllImages()
    {
        if (OddImages != null)
            for (int i = 0; i < OddImages.Length; i++)
                if (OddImages[i] != null) OddImages[i].gameObject.SetActive(false);

        if (EvenImages != null)
            for (int i = 0; i < EvenImages.Length; i++)
                if (EvenImages[i] != null) EvenImages[i].gameObject.SetActive(false);
    }

    // Khi discuss tắt -> chờ 5s -> xóa dữ liệu -> bật object phase tiếp theo
    private IEnumerator AdvanceAfterDiscussOff()
    {
        yield return new WaitForSeconds(5f);

        // Xóa sạch dữ liệu phase hiện tại
        displaySprites.Clear();
        killsThisPhase.Clear();

        // Bật object kế tiếp (nếu có)
        if (nextPhaseObject != null)
            nextPhaseObject.SetActive(true);

        // Hủy component để không chạy thêm ở phase này (giữ GameObject nếu bạn cần)
        Destroy(this);
    }

    // Giữ nguyên cơ chế: nếu có deadbody canvas bật -> sau 2.6s bật discuss
    private void CheckAIcanvasActivation()
    {
        if (aiDeathCanvases == null || aiDeathCanvases.Length == 0) return;

        for (int i = 0; i < aiDeathCanvases.Length; i++)
        {
            var canvasObj = aiDeathCanvases[i];
            if (canvasObj != null && canvasObj.activeSelf)
            {
                StartCoroutine(HandleCanvasSequence(canvasObj));
                break;
            }
        }
    }

    private IEnumerator HandleCanvasSequence(GameObject deadCanvas)
    {
        // delay như logic cũ
        yield return new WaitForSeconds(2.6f);

        if (deadCanvas != null)
            Destroy(deadCanvas);

        if (discussCanvas != null)
            discussCanvas.SetActive(true);

        // Khi discuss vừa bật, nếu Panel cũng đang bật thì sẽ show ở Update->ShowIfPanelReady()
    }
}
    