using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    // Tên Scene Menu trong Build Settings
    public string menuSceneName = "Menu";

    // Hàm này sẽ được gán cho OnClick của Button
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
