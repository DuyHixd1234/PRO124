using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FootstepZone : MonoBehaviour
{
    [Header("Footstep Clips for this zone")]
    public AudioClip[] zoneFootsteps;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyZoneFootsteps(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyZoneFootsteps(other); // đảm bảo vẫn đang dùng bộ âm zone
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerFootstepController footstepCtrl = other.GetComponent<PlayerFootstepController>();
        if (footstepCtrl != null)
        {
            footstepCtrl.ResetToDefault();
        }
    }

    private void ApplyZoneFootsteps(Collider2D other)
    {
        PlayerFootstepController footstepCtrl = other.GetComponent<PlayerFootstepController>();
        if (footstepCtrl != null && zoneFootsteps.Length > 0)
        {
            // Chỉ set nếu khác bộ hiện tại để tránh lặp vô ích
            if (footstepCtrl.CurrentClips != zoneFootsteps)
            {
                footstepCtrl.SetFootstepClips(zoneFootsteps);
            }
        }
    }
}
