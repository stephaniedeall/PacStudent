using UnityEngine;

public class PacStudentAudio : MonoBehaviour
{
    public AudioClip moveSound;
    public AudioClip pelletEatSound;
    public AudioClip wallCollisionSound;
    public AudioClip deathSound;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayMoveSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = moveSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    public void StopMoveSound()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
    
    public void PlayPelletEatSound()
    {
        audioSource.PlayOneShot(pelletEatSound);
    }
    
    public void PlayWallCollisionSound()
    {
        audioSource.PlayOneShot(wallCollisionSound);
    }
    
    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
}