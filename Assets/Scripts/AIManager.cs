using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    public GameObject[] aiList;
    public GameObject player;
    public GameObject discussPanel;
    public Transform[] spawnPoints;

    private bool isDiscussionOngoing = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AssignRoles();
        discussPanel?.SetActive(false);
    }

    void AssignRoles()
    {
        for (int i = 0; i < aiList.Length; i++)
        {
            bool isImp = PlayerPrefs.GetInt($"AI_{i}_IsImpostor", 0) == 1;
            AIBase ai = aiList[i].GetComponent<AIBase>();
            ai.role = isImp ? AIRole.Impostor : AIRole.Crewmate;
            ai.tag = ai.role == AIRole.Impostor ? "Impostor" : "Crewmate";
            Debug.Log($"AI {ai.aiName} gan role: {ai.role}");
        }

        if (player != null)
        {
            bool isPlayerImp = PlayerPrefs.GetInt("Player_IsImpostor", 0) == 1;
            AIBase ai = player.GetComponent<AIBase>();
            ai.role = isPlayerImp ? AIRole.Impostor : AIRole.Crewmate;
            ai.tag = ai.role == AIRole.Impostor ? "Impostor" : "Crewmate";
            Debug.Log($"PLAYER {ai.aiName} gan role: {ai.role}");
        }
    }

    public void ReportBody()
    {
        if (!isDiscussionOngoing)
        {
            Debug.Log("Co nguoi da bao cao Body!");
            Vector3 reportPosition = Vector3.zero;
            GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");
            if (bodies.Length > 0)
                reportPosition = bodies[0].transform.position;

            TriggerDiscussion(reportPosition);
        }
    }

    public void TriggerDiscussion(Vector3 point)
    {
        if (!isDiscussionOngoing)
            StartCoroutine(DiscussionRoutine(point));
    }

    IEnumerator DiscussionRoutine(Vector3 pos)
    {
        isDiscussionOngoing = true;
        Time.timeScale = 0f;
        discussPanel?.SetActive(true);
        Debug.Log("Bat dau thao luan, tam dung game.");

        for (int i = 0; i < aiList.Length; i++)
        {
            if (aiList[i] != null && aiList[i].activeInHierarchy)
            {
                AIBase ai = aiList[i].GetComponent<AIBase>();
                if (ai != null && !ai.isDead)
                {
                    ai.transform.position = spawnPoints[i].position;
                    Debug.Log($"Crew {ai.aiName} quay ve vi tri thao luan.");
                }
            }
        }

        if (player != null && player.activeInHierarchy)
        {
            AIBase ai = player.GetComponent<AIBase>();
            if (ai != null && !ai.isDead)
            {
                player.transform.position = spawnPoints[aiList.Length].position;
                Debug.Log($"Player {ai.aiName} quay ve vi tri thao luan.");
            }
        }

        GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");
        foreach (GameObject b in bodies)
        {
            b.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(5f);

        discussPanel?.SetActive(false);
        Time.timeScale = 1f;
        isDiscussionOngoing = false;
        Debug.Log("Ket thuc thao luan, tiep tuc game.");
    }

    public void CheckWinLose()
    {
        int impAlive = 0;
        int crewAlive = 0;

        // Danh sách impostor & crewmate AI còn sống
        string aliveImpostor = "";
        string aliveCrew = "";

        foreach (GameObject go in aiList)
        {
            AIBase ai = go.GetComponent<AIBase>();
            if (ai != null && !ai.isDead)
            {
                if (ai.role == AIRole.Impostor)
                {
                    impAlive++;
                    aliveImpostor += ai.aiName + ",";
                }
                else
                {
                    crewAlive++;
                    aliveCrew += ai.aiName + ",";
                }
            }
        }

        AIBase playerAI = player.GetComponent<AIBase>();
        if (!playerAI.isDead)
        {
            if (playerAI.role == AIRole.Impostor)
            {
                impAlive++;
                aliveImpostor += playerAI.aiName + ",";
            }
            else
            {
                crewAlive++;
                aliveCrew += playerAI.aiName + ",";
            }
        }

        // Ghi thông tin người chơi
        PlayerPrefs.SetInt("IsPlayerImpostor", playerAI.role == AIRole.Impostor ? 1 : 0);

        // Ghi danh sách AI còn sống để scene Win/Lose xử lý
        PlayerPrefs.SetString("AliveImpostor", aliveImpostor);
        PlayerPrefs.SetString("AliveCrew", aliveCrew);

        Debug.Log($"So Impostor con song: {impAlive}, Crewmate con song: {crewAlive}");

        if (impAlive == 0)
        {
            Debug.Log("Tat ca impostor da bi loai bo => WIN");
            SceneManager.LoadScene("Win");
        }
        else if (impAlive >= crewAlive)
        {
            Debug.Log("Impostor chiem uu the => LOSE");
            SceneManager.LoadScene("Lose");
        }
    }

}
