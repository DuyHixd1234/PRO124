using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public GameObject panelVictory;
    public GameObject panelDefeat;
    public Transform impostorImageParent;
    public Transform crewImageParent;
    public GameObject imagePrefab; // Image template

    public Sprite[] nameToSprite; // Sprite tương ứng với aiName (bạn có thể map theo index hoặc dictionary)

    void Start()
    {
        bool isPlayerImp = PlayerPrefs.GetInt("IsPlayerImpostor", 0) == 1;

        string[] aliveImps = PlayerPrefs.GetString("AliveImpostor", "").Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        string[] aliveCrews = PlayerPrefs.GetString("AliveCrew", "").Split(',', System.StringSplitOptions.RemoveEmptyEntries);

        bool isVictory = false;
        if (!isPlayerImp && aliveImps.Length == 0)
            isVictory = true;
        else if (isPlayerImp && aliveImps.Length >= aliveCrews.Length)
            isVictory = true;

        panelVictory.SetActive(isVictory);
        panelDefeat.SetActive(!isVictory);

        // Hiển thị ảnh các nhân vật tùy theo kết quả
        string[] toShow = isVictory ? (isPlayerImp ? aliveImps : aliveCrews) : (isPlayerImp ? aliveCrews : aliveImps);
        Transform parent = isVictory ? (isPlayerImp ? impostorImageParent : crewImageParent)
                                     : (isPlayerImp ? crewImageParent : impostorImageParent);

        foreach (string name in toShow)
        {
            GameObject img = Instantiate(imagePrefab, parent);
            Image imgComp = img.GetComponent<Image>();
            imgComp.sprite = GetSpriteByName(name);
        }
    }

    Sprite GetSpriteByName(string name)
    {
        // Giả sử sprite có tên giống aiName
        foreach (Sprite s in nameToSprite)
        {
            if (s.name == name)
                return s;
        }
        return null;
    }
}
