using UnityEngine;
using UnityEngine.UI;

public class EndSceneSpriteManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] AliveSprites;  // 10 nhân vật còn sống (0 = Player, 1-9 = AI)
    public Sprite[] GhostSprites;  // 10 nhân vật bóng ma tương ứng

    [Header("UI Target Images (Crewmate Canvas)")]
    public Image[] targetImages;   // 9 slot UI, index 0 = Player, 1-8 = AI còn lại (bỏ impostor)

    [Header("Ghost Alpha")]
    [Range(0f, 1f)] public float ghostAlpha = 0.7f; // độ mờ khi chết

    void Start()
    {
        int impostorIndex = PlayerPrefs.GetInt("ImpostorIndex", -1);

        if (impostorIndex == -1)
        {
            Debug.LogError("Không tìm thấy ImpostorIndex trong PlayerPrefs!");
            return;
        }

        int uiIndex = 0; // chỉ số targetImages

        for (int i = 0; i < 10; i++)
        {
            if (i == impostorIndex) continue; // bỏ impostor khỏi danh sách hiển thị

            if (uiIndex >= targetImages.Length)
            {
                Debug.LogWarning($"Số lượng targetImages ({targetImages.Length}) không đủ cho các crewmates.");
                break;
            }

            bool isDead = PlayerPrefs.GetInt($"Dead_Index_{i}", 0) == 1;

            if (i >= AliveSprites.Length || i >= GhostSprites.Length)
            {
                Debug.LogWarning($"Index sprite {i} vượt quá giới hạn mảng sprites.");
                SetImage(targetImages[uiIndex], null, 0f);
                uiIndex++;
                continue;
            }

            if (isDead)
                SetImage(targetImages[uiIndex], GhostSprites[i], ghostAlpha);
            else
                SetImage(targetImages[uiIndex], AliveSprites[i], 1f);

            uiIndex++;
        }
    }

    void SetImage(Image img, Sprite sprite, float alpha)
    {
        if (img == null)
        {
            Debug.LogWarning("SetImage thất bại: Image là null");
            return;
        }

        img.sprite = sprite;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
