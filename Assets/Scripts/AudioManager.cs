using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioClip introMusic;
    public AudioClip normalStateMusic;
    public AudioClip scaredStateMusic;
    public AudioClip deadStateMusic;
    
    private AudioSource musicSource;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
            
        DontDestroyOnLoad(gameObject);
        
        musicSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        PlayIntroMusic();
    }
    
    public void PlayIntroMusic()
    {
        StartCoroutine(PlayIntroSequence());
    }
    
    IEnumerator PlayIntroSequence()
    {
        musicSource.clip = introMusic;
        musicSource.Play();
        
        float introDuration = Mathf.Min(introMusic.length, 3f);
        yield return new WaitForSeconds(introDuration);
        
        PlayNormalMusic();
    }
    
    public void PlayNormalMusic()
    {
        musicSource.clip = normalStateMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlayScaredMusic()
    {
        musicSource.clip = scaredStateMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlayDeadMusic()
    {
        musicSource.clip = deadStateMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}
