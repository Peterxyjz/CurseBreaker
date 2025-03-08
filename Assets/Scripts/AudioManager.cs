using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [SerializeField] private AudioClip backgroundClip;
    [SerializeField] private AudioClip attackSword;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip collectSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySwordAttack()
    {
        effectAudioSource.PlayOneShot(attackSword);
    }
    public void PlayPlayerJump()
    {
        effectAudioSource.PlayOneShot(jumpSound);
    }
    public void PlayCollectionItem()
    {
        effectAudioSource.PlayOneShot(collectSound);
    }
    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backgroundClip;
        backgroundAudioSource.Play();
    }
}
