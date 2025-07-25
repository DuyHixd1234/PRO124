using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Transform[] nextWaypoints;

    public Transform GetRandomNext()
    {
        if (nextWaypoints.Length == 0) return null;
        return nextWaypoints[Random.Range(0, nextWaypoints.Length)];
    }
}
