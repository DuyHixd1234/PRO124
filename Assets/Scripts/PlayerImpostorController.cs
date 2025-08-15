using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerImpostorController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button killButton;

    [Header("Canvas Group")]
    public CanvasGroup killButtonGroup;

    [Header("Detection")]
    public Transform detectZone;
    private AICrewmate targetCrew;

    [Header("Kill Circle Object")]
    public GameObject killCircle;
    public float killActiveTime = 0.3f;

    void Start()
    {
        SafeSetKillInteractable(false);

        if (killButton != null)
            killButton.onClick.AddListener(HandleKill);

        if (killCircle != null)
            killCircle.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate"))
        {
            targetCrew = collision.GetComponent<AICrewmate>();
            if (targetCrew != null && targetCrew.gameObject.activeSelf)
            {
                SafeSetKillInteractable(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Crewmate") && targetCrew != null && collision.gameObject == targetCrew.gameObject)
        {
            targetCrew = null;
            SafeSetKillInteractable(false);
        }
    }

    void HandleKill()
    {
        if (targetCrew == null) return;

        if (killCircle != null)
            StartCoroutine(ActivateKillCircle());
    }

    private System.Collections.IEnumerator ActivateKillCircle()
    {
        killCircle.SetActive(true);
        yield return new WaitForSeconds(killActiveTime);
        killCircle.SetActive(false);
    }

    void SafeSetKillInteractable(bool state)
    {
        if (killButton != null)
            killButton.interactable = state;

        if (killButtonGroup != null)
            killButtonGroup.alpha = state ? 1f : 0.4f;
    }
}
