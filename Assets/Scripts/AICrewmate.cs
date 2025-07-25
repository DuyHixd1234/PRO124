using UnityEngine;

public class AICrewmate : MonoBehaviour
{
    public GameObject bodyStanding;  // SetActive(true) 1s
    public GameObject bodyFallen;    // SetActive(true) sau ?ó
    public GameObject shadow;        // Tat khi chet

    public void Kill()
    {
        if (shadow != null) shadow.SetActive(false);
        StartCoroutine(ShowDeadBody());
    }

    System.Collections.IEnumerator ShowDeadBody()
    {
        if (bodyStanding != null) bodyStanding.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (bodyStanding != null) bodyStanding.SetActive(false);
        if (bodyFallen != null) bodyFallen.SetActive(true);

        gameObject.SetActive(false);
    }
}
