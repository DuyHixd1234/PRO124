using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public GameObject panelMenu;   // Panel menu dang hien
    public GameObject panelStart;  // Panel start dang an luc dau

    public void OnClickStartButton()
    {
        if (panelMenu != null)
            panelMenu.SetActive(false);

        if (panelStart != null)
            panelStart.SetActive(true);
    }
}
