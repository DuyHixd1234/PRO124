using TMPro;
using UnityEngine;

public class AIVisionDebug : MonoBehaviour
{
    public TMP_Text nameText;

    void Start()
    {
        int index = GetAIIndexFromName();
        string name = PlayerPrefs.GetString($"AI_{index}_Name", $"AI_{index}");
        bool isImp = PlayerPrefs.GetInt($"AI_{index}_IsImpostor", 0) == 1;

        nameText.text = name + "\n" + (isImp ? "Impostor" : "Crewmate");
        nameText.color = isImp ? Color.red : Color.cyan;
    }

    int GetAIIndexFromName()
    {
        string name = gameObject.name;
        if (name.StartsWith("AI_"))
        {
            string indexStr = name.Substring(3);
            int.TryParse(indexStr, out int index);
            return index;
        }
        return 0;
    }
}
