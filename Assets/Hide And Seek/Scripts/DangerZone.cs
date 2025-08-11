using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DangerZone2D : MonoBehaviour
{
    [HideInInspector] public int zoneIndex;
    [HideInInspector] public Danger parentDanger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Impostor"))
        {
            parentDanger?.SetCircleDetected(zoneIndex, true);
            Debug.Log($"[Zone {zoneIndex + 1}] Impostor ENTER - {other.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Impostor"))
        {
            parentDanger?.SetCircleDetected(zoneIndex, false);
            Debug.Log($"[Zone {zoneIndex + 1}] Impostor EXIT - {other.name}");
        }
    }
}
