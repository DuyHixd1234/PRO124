using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button startButton;
    public GameObject fadePanelGO;
    public Image fadePanelImage;

    void Start()
    {
        startButton.interactable = false;
        fadePanelGO.SetActive(false);
        PlayerPrefs.DeleteKey("VotedOutIndex");
        for (int i = 0; i < 9; i++)
            PlayerPrefs.DeleteKey($"AI_{i}_IsDead");
    }

    void Update()
    {
        startButton.interactable = nameInput.text.Length > 0;
    }

    public void OnStartGame()
    {
        if (PlayerData.Instance == null)
        {
            GameObject data = new GameObject("PlayerData");
            data.AddComponent<PlayerData>();
        }

        PlayerData.Instance.playerName = nameInput.text;

        startButton.interactable = false;
        StartCoroutine(FadeAndLoadScene("Lobby"));
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        fadePanelGO.SetActive(true);
        float duration = 0.75f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            Color color = fadePanelImage.color;
            color.a = alpha;
            fadePanelImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}
