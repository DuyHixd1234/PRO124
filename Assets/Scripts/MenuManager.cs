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

    private const string playerNameKey = "PlayerName";
    private const string defaultName = "Player";

    private void Start()
    {
        // Luôn bật Start
        startButton.interactable = true;
        fadePanelGO.SetActive(false);

        // Xoá dữ liệu cũ (gameplay)
        PlayerPrefs.DeleteKey("VotedOutIndex");
        for (int i = 0; i < 9; i++)
            PlayerPrefs.DeleteKey($"AI_{i}_IsDead");

        // Load tên cũ nếu có
        string savedName = PlayerPrefs.GetString(playerNameKey, defaultName);
        nameInput.placeholder.GetComponent<TMP_Text>().text = savedName;

        // Tự động focus input field
        nameInput.ActivateInputField();
    }

    private void Update()
    {
        // Giới hạn 20 ký tự (không tính dấu cách)
        string raw = nameInput.text;
        string noSpace = raw.Replace(" ", "");
        if (noSpace.Length > 20)
        {
            int validCount = 0;
            string result = "";
            foreach (char c in raw)
            {
                if (c != ' ') validCount++;
                if (validCount > 20) break;
                result += c;
            }
            nameInput.text = result;
            nameInput.caretPosition = result.Length;
        }

        // Nếu đang focus và bấm Enter thì cũng start game
        if (nameInput.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            OnStartGame();
        }
    }

    public void OnStartGame()
    {
        if (PlayerData.Instance == null)
        {
            GameObject data = new GameObject("PlayerData");
            data.AddComponent<PlayerData>();
        }

        string finalName = nameInput.text.Trim();

        // Nếu input rỗng → lấy PlayerPrefs (nếu có) hoặc "Player"
        if (string.IsNullOrEmpty(finalName))
            finalName = PlayerPrefs.GetString(playerNameKey, defaultName);

        PlayerData.Instance.playerName = finalName;

        // Lưu vào PlayerPrefs
        PlayerPrefs.SetString(playerNameKey, finalName);
        PlayerPrefs.Save();

        Debug.Log("[MenuManager] Player Name sử dụng: " + finalName);

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
