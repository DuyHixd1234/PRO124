using UnityEngine;

public class ColorSelect : MonoBehaviour
{
    public GameObject redPlayer;
    public GameObject yellowPlayer;
    public GameObject darkGreenPlayer;
    public GameObject whitePlayer;

    void Start()
    {
        int colorIndex = PlayerData.Instance.selectedColorIndex;

        redPlayer.SetActive(colorIndex == 0);
        yellowPlayer.SetActive(colorIndex == 1);
        darkGreenPlayer.SetActive(colorIndex == 2);
        whitePlayer.SetActive(colorIndex == 3);
    }
}
