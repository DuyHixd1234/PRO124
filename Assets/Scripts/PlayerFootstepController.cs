using UnityEngine;

public class PlayerFootstepController : MonoBehaviour
{
    [Header("Footstep Clips")]
    public AudioClip[] defaultFootsteps;
    private AudioClip[] currentFootsteps;

    [Header("Settings")]
    public float stepInterval = 0.35f; // thời gian giữa 2 bước
    public AudioSource footstepSource;

    private Player playerScript;
    private float stepTimer;
    public AudioClip[] CurrentClips { get; private set; }
    void Start()
    {
        playerScript = GetComponent<Player>();

        if (playerScript == null)
        {
            Debug.LogError("[Footstep] Không tìm thấy script Player trên object!");
        }

        currentFootsteps = defaultFootsteps;

        if (footstepSource == null)
            Debug.LogError("[Footstep] Chưa gán AudioSource!");

        if (defaultFootsteps == null || defaultFootsteps.Length == 0)
            Debug.LogError("[Footstep] Chưa gán defaultFootsteps!");
    }

    void Update()
    {
        // Lấy movement vector từ Player lol
        Vector2 movement = playerScript != null ? playerScript.movement : Vector2.zero;

        bool isMoving = movement.magnitude > 0.05f && !playerScript.anim.GetBool("isRunning") == false;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayRandomFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayRandomFootstep()
    {
        if (currentFootsteps != null && currentFootsteps.Length > 0)
        {
            AudioClip clip = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
            footstepSource.PlayOneShot(clip);
            // Debug.Log("[Footstep] Play: " + clip.name);
        }
    }

    public void SetFootstepClips(AudioClip[] clips)
    {
        CurrentClips = clips;
        currentFootsteps = clips;
    }

    public void ResetToDefault()
    {
        CurrentClips = defaultFootsteps;
        currentFootsteps = defaultFootsteps;
    }
}
