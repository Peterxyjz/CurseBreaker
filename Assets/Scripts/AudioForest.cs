using UnityEngine;

public class AudioForest : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudio;
    [SerializeField] private AudioClip lighting;
    [SerializeField] private AudioClip barricade;
    [SerializeField] private AudioClip slimeDie;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayLighting()
    {
        effectAudio.PlayOneShot(lighting);
    }
    public void PlayBarricade()
    {
        effectAudio.PlayOneShot(barricade);
    }
    public void PlaySlimeDie()
    {
        effectAudio.PlayOneShot(slimeDie);
    }
}
