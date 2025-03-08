using UnityEngine;

public class AudioBoss : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudio;
    [SerializeField] private AudioClip normalSkillClip;
    [SerializeField] private AudioClip skillNo1Clip;
    [SerializeField] private AudioClip breakerClip;
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip dieClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayNormalSkill()
    {
        effectAudio.PlayOneShot(normalSkillClip);
    }
    public void PlaySkillNo1Clip()
    {
        effectAudio.PlayOneShot(skillNo1Clip);
    }
    public void PlayBreakerClip()
    {
        effectAudio.PlayOneShot(breakerClip);
    }
    public void PlayIdlerClip()
    {
        effectAudio.PlayOneShot(idleClip);
    }
    public void PlayDieClip()
    {
        effectAudio.PlayOneShot(idleClip);
    }
}
