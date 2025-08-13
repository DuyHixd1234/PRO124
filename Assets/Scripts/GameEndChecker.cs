using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameEndChecker : MonoBehaviour
{
    [Header("Các phần tử cần check (10 elements)")]
    public GameObject[] elements = new GameObject[10]; // Gán đủ 10

    [Header("Canvas cần kiểm tra trước khi chuyển scene")]
    public GameObject[] canvasList = new GameObject[9]; // Gán 9 canvas discuss/deadbody

    [Header("UI thông báo Win/Lose")]
    public GameObject uiObject; // Bật khi sắp chuyển scene

    [Header("Thời gian delay bắt đầu check Win/Lose (giây)")]
    public float startCheckDelay = 15f;

    [Header("Thời gian delay trước khi chuyển scene")]
    public float delayBeforeSceneChange = 1f;

    private bool isEnding = false;
    private float startCheckTimer;

    private HashSet<int> aliveIndexImpostor = new HashSet<int>();
    private HashSet<int> aliveIndexCrewmate = new HashSet<int>();
    private HashSet<int> deadIndexImpostor = new HashSet<int>();
    private HashSet<int> deadIndexCrewmate = new HashSet<int>();

    private bool[] prevActiveState = new bool[10];

    void Start()
    {
        startCheckTimer = startCheckDelay;

        for (int i = 0; i < elements.Length; i++)
        {
            prevActiveState[i] = elements[i] != null && elements[i].activeSelf;
            TrackInitialState(i);
        }
        SaveDataToPrefs();
    }

    void Update()
    {
        if (isEnding) return;

        if (startCheckTimer > 0f)
        {
            startCheckTimer -= Time.deltaTime;
            return;
        }

        UpdateAliveDeadLists();
        SaveDataToPrefs();

        // Nếu elements[0] null hoặc inactive => Lose
        if (elements[0] == null || !elements[0].activeSelf)
        {
            Debug.Log("Element 0 null hoặc inactive → Lose");
            TriggerLose();
            return;
        }

        // Đếm lại chỉ những object còn sống (active) và có tag hợp lệ
        int impostorCount = 0;
        int crewmateCount = 0;
        foreach (var obj in elements)
        {
            if (obj != null && obj.activeSelf)
            {
                if (obj.CompareTag("Impostor")) impostorCount++;
                else if (obj.CompareTag("Crewmate")) crewmateCount++;
            }
        }

        // Nếu tất cả object active đều Untagged => bỏ qua
        if (impostorCount == 0 && crewmateCount == 0)
        {
            Debug.Log("Tất cả object đang active đều Untagged → bỏ qua");
            return;
        }

        string role0 = elements[0].tag;

        // Điều kiện thắng/thua
        if (impostorCount == 0)
        {
            if (role0 == "Crewmate") TriggerWin();
            else if (role0 == "Impostor") TriggerLose();
            return;
        }

        if (impostorCount >= crewmateCount)
        {
            if (role0 == "Impostor") TriggerWin();
            else if (role0 == "Crewmate") TriggerLose();
            return;
        }
    }

    private void TrackInitialState(int index)
    {
        if (elements[index] == null) return;

        if (elements[index].CompareTag("Impostor"))
            aliveIndexImpostor.Add(index);
        else if (elements[index].CompareTag("Crewmate"))
            aliveIndexCrewmate.Add(index);
    }

    private void UpdateAliveDeadLists()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            bool isActive = elements[i] != null && elements[i].activeSelf;

            // Object vừa bị destroy hoặc tắt
            if (prevActiveState[i] && !isActive)
            {
                if (aliveIndexImpostor.Remove(i))
                    deadIndexImpostor.Add(i);
                else if (aliveIndexCrewmate.Remove(i))
                    deadIndexCrewmate.Add(i);
            }

            // Object vừa bật lại
            if (!prevActiveState[i] && isActive && elements[i] != null)
            {
                if (elements[i].CompareTag("Impostor"))
                {
                    deadIndexImpostor.Remove(i);
                    aliveIndexImpostor.Add(i);
                }
                else if (elements[i].CompareTag("Crewmate"))
                {
                    deadIndexCrewmate.Remove(i);
                    aliveIndexCrewmate.Add(i);
                }
            }

            prevActiveState[i] = isActive;
        }
    }

    private void SaveDataToPrefs()
    {
        PlayerPrefs.SetInt("AliveImpostorCount", aliveIndexImpostor.Count);
        int idx = 0;
        foreach (var i in aliveIndexImpostor)
            PlayerPrefs.SetInt("AliveImpostorIndex_" + idx++, i);

        PlayerPrefs.SetInt("DeadImpostorCount", deadIndexImpostor.Count);
        idx = 0;
        foreach (var i in deadIndexImpostor)
            PlayerPrefs.SetInt("DeadImpostorIndex_" + idx++, i);

        PlayerPrefs.SetInt("AliveCrewmateCount", aliveIndexCrewmate.Count);
        idx = 0;
        foreach (var i in aliveIndexCrewmate)
            PlayerPrefs.SetInt("AliveCrewmateIndex_" + idx++, i);

        PlayerPrefs.SetInt("DeadCrewmateCount", deadIndexCrewmate.Count);
        idx = 0;
        foreach (var i in deadIndexCrewmate)
            PlayerPrefs.SetInt("DeadCrewmateIndex_" + idx++, i);

        PlayerPrefs.Save();
    }

    private void TriggerWin()
    {
        if (isEnding) return;
        isEnding = true;
        StartCoroutine(WaitForCanvasAndChangeScene("Win"));
    }

    private void TriggerLose()
    {
        if (isEnding) return;
        isEnding = true;
        StartCoroutine(WaitForCanvasAndChangeScene("Lose"));
    }

    private IEnumerator WaitForCanvasAndChangeScene(string sceneName)
    {
        yield return new WaitUntil(() => AllCanvasOff());

        if (uiObject != null) uiObject.SetActive(true);
        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene(sceneName);
    }

    private bool AllCanvasOff()
    {
        foreach (GameObject canvas in canvasList)
        {
            if (canvas != null && canvas.activeSelf) return false;
        }
        return true;
    }
}
