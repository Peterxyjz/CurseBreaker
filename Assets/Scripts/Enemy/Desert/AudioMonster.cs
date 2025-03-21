using UnityEngine;

public class AudioMonster : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudio;
    [SerializeField] private AudioClip skeletonDie;
    [SerializeField] private AudioClip skeletonRun;

    [SerializeField] private AudioClip archerAttack;

    [SerializeField] private AudioClip archerDie;
    [SerializeField] private AudioClip bossAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartSkeletonRunLoop()
    {
        effectAudio.clip = skeletonRun;
        effectAudio.loop = true;
        effectAudio.Play();
    }

    public void StopSkeletonRunLoop()
    {
        effectAudio.Stop();
        effectAudio.loop = false;
    }
    public void PlayerSkeletonDie()
    {
        effectAudio.PlayOneShot(skeletonDie);
    }
    public void PlayerArcherAttack()
    {
        effectAudio.PlayOneShot(archerAttack);
    }
    public void PlayerArcherDie()
    {
        effectAudio.PlayOneShot(archerDie);
    }
    public void PlayerBossAttack()
    {
        effectAudio.PlayOneShot(bossAttack);
    }
}
