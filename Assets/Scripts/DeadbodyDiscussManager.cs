using UnityEngine;
using System.Collections;

public class DeadbodyDiscussManager : MonoBehaviour
{
    [Header("9 Canvas Deadbody (ẩn sẵn)")]
    public GameObject[] deadbodyCanvases; // Size = 9

    [Header("9 Canvas Discuss (ẩn sẵn)")]
    public GameObject[] discussCanvases; // Size = 9

    private bool[] isDeadbodyTriggered; // Để tránh check lặp
    private bool[] isDiscussActive;     // Trạng thái discuss đang bật

    void Start()
    {
        // Bảo đảm mảng an toàn
        int length = Mathf.Min(deadbodyCanvases.Length, discussCanvases.Length);
        isDeadbodyTriggered = new bool[length];
        isDiscussActive = new bool[length];

        // Tắt tất cả Discuss ngay từ đầu
        for (int i = 0; i < discussCanvases.Length; i++)
        {
            if (discussCanvases[i] != null)
                discussCanvases[i].SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < deadbodyCanvases.Length; i++)
        {
            // Nếu Deadbody đã bị destroy thì bỏ qua
            if (deadbodyCanvases[i] == null) continue;

            bool isActiveNow = deadbodyCanvases[i].activeSelf;

            // Phát hiện Deadbody vừa được bật lần đầu
            if (isActiveNow && !isDeadbodyTriggered[i])
            {
                isDeadbodyTriggered[i] = true;
                StartCoroutine(ActivateDiscussAfterDelay(i, 2.5f));
            }

            // Nếu Discuss đã từng bật, kiểm tra khi nó tắt → destroy
            if (isDiscussActive[i])
            {
                if (discussCanvases[i] == null || !discussCanvases[i].activeSelf)
                {
                    // Destroy cả discuss và deadbody (nếu còn tồn tại)
                    if (discussCanvases[i] != null)
                    {
                        Destroy(discussCanvases[i]);
                        discussCanvases[i] = null;
                    }

                    if (deadbodyCanvases[i] != null)
                    {
                        Destroy(deadbodyCanvases[i]);
                        deadbodyCanvases[i] = null;
                    }

                    isDiscussActive[i] = false;
                }
            }
        }
    }

    private IEnumerator ActivateDiscussAfterDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (index >= discussCanvases.Length) yield break;
        if (discussCanvases[index] != null)
        {
            discussCanvases[index].SetActive(true);
            isDiscussActive[index] = true;
        }
    }
}
