using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject panelToHide;
    public GameObject panelToShow;

    public void SwitchPanel()
    {
        if (panelToHide != null)
            panelToHide.SetActive(false);

        if (panelToShow != null)
            panelToShow.SetActive(true);
    }
}
