using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;
    public AudioClip typingSound;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayTypingSound()
    {
        if (typingSound != null && audioSource != null)
            audioSource.PlayOneShot(typingSound);
    }
}
