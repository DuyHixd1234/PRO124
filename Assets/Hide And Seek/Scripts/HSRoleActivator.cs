using UnityEngine;

public class HSRoleActivator : MonoBehaviour
{
    [Header("Crewmate Objects")]
    public GameObject redCrewmate;
    public GameObject yellowCrewmate;
    public GameObject greenCrewmate;
    public GameObject whiteCrewmate;

    [Header("Impostor Objects")]
    public GameObject redImpostor;
    public GameObject yellowImpostor;
    public GameObject greenImpostor;
    public GameObject whiteImpostor;

    private void Start()
    {
        // Lấy màu của Human
        int colorIndex = PlayerData.Instance.selectedColorIndex;

        // Lấy role từ HSShuffle
        int impostorIndex = PlayerPrefs.GetInt("HideSeek_ImpostorIndex", -1);

        GameObject chosenObject = null;

        if (impostorIndex == 0) // Human là Impostor
        {
            chosenObject = GetImpostorObject(colorIndex);
            DestroyCrewmateObjects(); // Xóa toàn bộ Crewmate
        }
        else // Human là Crewmate
        {
            chosenObject = GetCrewmateObject(colorIndex);
            DestroyImpostorObjects(); // Xóa toàn bộ Impostor
        }

        if (chosenObject != null)
        {
            chosenObject.SetActive(true);
            chosenObject.tag = "Crewmate"; // theo yêu cầu, luôn tag là Crewmate
        }
        else
        {
            Debug.LogWarning("[HSRoleActivator] Không tìm thấy object phù hợp!");
        }
    }

    private void DestroyCrewmateObjects()
    {
        DestroyIfNotNull(redCrewmate);
        DestroyIfNotNull(yellowCrewmate);
        DestroyIfNotNull(greenCrewmate);
        DestroyIfNotNull(whiteCrewmate);
    }

    private void DestroyImpostorObjects()
    {
        DestroyIfNotNull(redImpostor);
        DestroyIfNotNull(yellowImpostor);
        DestroyIfNotNull(greenImpostor);
        DestroyIfNotNull(whiteImpostor);
    }

    private void DestroyIfNotNull(GameObject obj)
    {
        if (obj != null)
            Destroy(obj);
    }

    private GameObject GetCrewmateObject(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0: return redCrewmate;
            case 1: return yellowCrewmate;
            case 2: return greenCrewmate;
            case 3: return whiteCrewmate;
            default: return null;
        }
    }

    private GameObject GetImpostorObject(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0: return redImpostor;
            case 1: return yellowImpostor;
            case 2: return greenImpostor;
            case 3: return whiteImpostor;
            default: return null;
        }
    }
}
