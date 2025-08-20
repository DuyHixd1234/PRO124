using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSeekPingManager : MonoBehaviour
{
    [Header("References")]
    public GameObject impostorCanvas;   // Gán Canvas Impostor ở đây
    public GameObject phaseSlider;      // Gán Slider (hoặc GO chứa Slider) ở đây
    public List<GameObject> pingObjects; // Gán 9 Ping (ẩn sẵn) vào list

    [Header("Timing")]
    public float pingDuration = 3f;     // Thời gian hiện ping
    public float cooldownDuration = 5f; // Thời gian chờ trước khi hiện lại

    private Coroutine pingRoutine;

    void OnEnable()
    {
        HideAllPings();
    }

    void Update()
    {
        // Nếu ImpostorCanvas hoặc Slider không bật => dừng hẳn routine
        if (impostorCanvas == null || phaseSlider == null)
            return;

        bool canPing = impostorCanvas.activeInHierarchy && phaseSlider.activeInHierarchy;

        if (canPing && pingRoutine == null)
        {
            pingRoutine = StartCoroutine(PingLoop());
        }
        else if (!canPing && pingRoutine != null)
        {
            StopCoroutine(pingRoutine);
            pingRoutine = null;
            HideAllPings();
        }
    }

    IEnumerator PingLoop()
    {
        while (true)
        {
            // Hiện pings
            ShowAllPings();
            yield return new WaitForSeconds(pingDuration);

            // Ẩn pings
            HideAllPings();
            yield return new WaitForSeconds(cooldownDuration);
        }
    }

    void ShowAllPings()
    {
        foreach (var ping in pingObjects)
        {
            if (ping != null) ping.SetActive(true);
        }
    }

    void HideAllPings()
    {
        foreach (var ping in pingObjects)
        {
            if (ping != null) ping.SetActive(false);
        }
    }
}
