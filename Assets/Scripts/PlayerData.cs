using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    [HideInInspector] public string playerName;
    [HideInInspector] public Color selectedColor;
    [HideInInspector] public int selectedColorIndex;
    [HideInInspector] public GameObject playerPrefab;
    [HideInInspector] public GameObject deadBodyPrefab;
    [HideInInspector] public bool isImpostor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
