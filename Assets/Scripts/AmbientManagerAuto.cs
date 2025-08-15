using UnityEngine;

[System.Serializable]
public class AmbientRoomAuto
{
    public string roomName;
    public AudioSource audioSource;
    public PolygonCollider2D polyCollider;
    [HideInInspector] public Vector2 centerPoint;
    [HideInInspector] public float maxDistance;
}

public class AmbientManagerAuto : MonoBehaviour
{
    public AudioSource mainAmbient;
    public AmbientRoomAuto[] rooms;
    public PolygonCollider2D[] muteZones; // vùng tắt toàn bộ âm thanh
    public float fadeSpeed = 2f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Setup main ambient
        if (mainAmbient != null)
        {
            mainAmbient.loop = true;
            mainAmbient.Play();
        }

        // Setup từng phòng
        foreach (var room in rooms)
        {
            if (room.audioSource != null)
            {
                room.audioSource.loop = true;
                room.audioSource.Play();
                room.audioSource.volume = 0f;
            }

            if (room.polyCollider != null)
            {
                // Tính tâm phòng
                Vector2 sum = Vector2.zero;
                foreach (var p in room.polyCollider.points)
                {
                    Vector2 worldP = room.polyCollider.transform.TransformPoint(p);
                    sum += worldP;
                }
                room.centerPoint = sum / room.polyCollider.points.Length;

                // Tính bán kính fade
                float maxDist = 0f;
                foreach (var p in room.polyCollider.points)
                {
                    Vector2 worldP = room.polyCollider.transform.TransformPoint(p);
                    float dist = Vector2.Distance(room.centerPoint, worldP);
                    if (dist > maxDist) maxDist = dist;
                }
                room.maxDistance = maxDist;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // Nếu đang trong vùng mute → tắt tất cả âm
        if (IsInMuteZone())
        {
            SetAllVolumes(0f);
            return;
        }

        float highestRoomVolume = 0f;

        // Cập nhật volume cho từng phòng
        foreach (var room in rooms)
        {
            if (room.polyCollider == null || room.audioSource == null) continue;

            float dist = Vector2.Distance(player.position, room.centerPoint);
            float targetVol = Mathf.Clamp01(1f - (dist / room.maxDistance));

            room.audioSource.volume = Mathf.Lerp(
                room.audioSource.volume,
                targetVol,
                Time.deltaTime * fadeSpeed
            );

            if (room.audioSource.volume > highestRoomVolume)
                highestRoomVolume = room.audioSource.volume;
        }

        // Cập nhật volume cho ambient chính
        if (mainAmbient != null)
        {
            float targetMainVol = 1f - highestRoomVolume;
            mainAmbient.volume = Mathf.Lerp(
                mainAmbient.volume,
                targetMainVol,
                Time.deltaTime * fadeSpeed
            );
        }
    }

    bool IsInMuteZone()
    {
        foreach (var zone in muteZones)
        {
            if (zone != null && zone.OverlapPoint(player.position))
                return true;
        }
        return false;
    }

    void SetAllVolumes(float vol)
    {
        if (mainAmbient != null)
            mainAmbient.volume = Mathf.Lerp(mainAmbient.volume, vol, Time.deltaTime * fadeSpeed);

        foreach (var room in rooms)
        {
            if (room.audioSource != null)
                room.audioSource.volume = Mathf.Lerp(room.audioSource.volume, vol, Time.deltaTime * fadeSpeed);
        }
    }
}
