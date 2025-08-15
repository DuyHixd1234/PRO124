using UnityEngine;
using UnityEngine.UI;

public class EmergencyMeetingTrigger : MonoBehaviour
{
    [SerializeField] private Button useButton;
    [SerializeField] private GameObject panelToShow;

    private void Start()
    {
        if (useButton != null)
        {
            useButton.interactable = false; // Ban đầu tắt
            useButton.onClick.AddListener(OnUseButtonClicked);
        }

        if (panelToShow != null)
        {
            panelToShow.SetActive(false); // Panel ẩn ban đầu
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (useButton != null)
                useButton.interactable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (useButton != null)
                useButton.interactable = false;
        }
    }

    private void OnUseButtonClicked()
    {
        if (useButton != null && useButton.interactable)
        {
            if (panelToShow != null)
                panelToShow.SetActive(true);
        }
    }
}
