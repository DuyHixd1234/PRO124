using UnityEngine;
using UnityEngine.UI;

public class VotingLockoutManager : MonoBehaviour
{
    public static void LockAllButtonsExcept(GameObject except)
    {
        // Dùng FindObjectsByType để tránh cảnh báo
        VotingButton[] buttons = GameObject.FindObjectsByType<VotingButton>(FindObjectsSortMode.None);
        foreach (var b in buttons)
        {
            if (b.gameObject != except)
            {
                b.buttonSelf.interactable = false;
            }
        }

        // Disable Skip button nếu có
        GameObject skip = GameObject.Find("ButtonSkip");
        if (skip != null)
        {
            Button skipBtn = skip.GetComponent<Button>();
            if (skipBtn != null) skipBtn.interactable = false;
        }
    }
}
