using UnityEngine;
using UnityEngine.UI;

public class ReportButtonHandler : MonoBehaviour
{
    [Header("Button Report")]
    public Button crewReportButton;
    public Button impostorReportButton;

    [Header("Deadbody Canvases")]
    public GameObject deadbodyBlue;
    public GameObject deadbodyCoral;
    public GameObject deadbodyOrange;
    public GameObject deadbodyBrown;
    public GameObject deadbodyLime;
    public GameObject deadbodyPink;
    public GameObject deadbodyCyan;
    public GameObject deadbodyPurple;
    public GameObject deadbodyGray;

    private string currentBodyTag = "";

    void Start()
    {
        // Disable cả hai nút từ đầu
        crewReportButton.interactable = false;
        impostorReportButton.interactable = false;

        // Gán chung 1 sự kiện OnClick cho cả hai nút
        crewReportButton.onClick.AddListener(ReportCurrentBody);
        impostorReportButton.onClick.AddListener(ReportCurrentBody);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered with: " + collision.tag);

        if (IsBodyTag(collision.tag))
        {
            currentBodyTag = collision.tag;

            if (crewReportButton != null)
                crewReportButton.interactable = true;
            if (impostorReportButton != null)
                impostorReportButton.interactable = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == currentBodyTag)
        {
            currentBodyTag = "";
            crewReportButton.interactable = false;
            impostorReportButton.interactable = false;
        }
    }

    private void ReportCurrentBody()
    {
        switch (currentBodyTag)
        {
            case "Blue": deadbodyBlue.SetActive(true); break;
            case "Coral": deadbodyCoral.SetActive(true); break;
            case "Orange": deadbodyOrange.SetActive(true); break;
            case "Brown": deadbodyBrown.SetActive(true); break;
            case "Lime": deadbodyLime.SetActive(true); break;
            case "Pink": deadbodyPink.SetActive(true); break;
            case "Cyan": deadbodyCyan.SetActive(true); break;
            case "Purple": deadbodyPurple.SetActive(true); break;
            case "Gray": deadbodyGray.SetActive(true); break;
        }

        crewReportButton.interactable = false;
        impostorReportButton.interactable = false;
        currentBodyTag = "";
    }

    private bool IsBodyTag(string tag)
    {
        return tag == "Blue" || tag == "Coral" || tag == "Orange" ||
               tag == "Brown" || tag == "Lime" || tag == "Pink" ||
               tag == "Cyan" || tag == "Purple" || tag == "Gray";
    }
}
