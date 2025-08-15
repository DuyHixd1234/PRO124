using UnityEngine;

public class QuitButtonAndroid : MonoBehaviour
{
    public void QuitApp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
