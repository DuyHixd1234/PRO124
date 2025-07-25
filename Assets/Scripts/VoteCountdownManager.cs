using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VoteCountdownManager : MonoBehaviour
{
    public TMP_Text tmpCount; // Text đếm: 5 → 4 → 3...
    public float startTime = 5f;

    void OnEnable()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        float timeLeft = startTime;
        while (timeLeft > 0)
        {
            tmpCount.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        SceneManager.LoadScene("VoteResult");
    }
}
