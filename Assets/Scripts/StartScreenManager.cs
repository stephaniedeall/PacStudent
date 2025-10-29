using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [Header("Button References")]
    public Button level1Button;
    public Button level2Button;
    
    [Header("Audio References")]
    public AudioSource backgroundMusic;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    
    [Header("High Score Display")]
    public Text level1HighScoreText;
    public Text level1BestTimeText;
    public Text level2HighScoreText;
    public Text level2BestTimeText;

    void Start()
    {
        level1Button.onClick.AddListener(LoadLevel1);
        level2Button.onClick.AddListener(LoadLevel2);
        
        SetupButtonHoverEffects();
        
        InitializeHighScores();
        
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }
    
    void LoadLevel1()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("Assessment3-Scene"); 
    }
    
    void LoadLevel2()
    {
        PlayButtonClickSound();
        Debug.Log("Level 2 button pressed - feature not implemented yet");
    }
    
    void SetupButtonHoverEffects()
{
    level1Button.onClick.AddListener(PlayButtonClickSound);
    level2Button.onClick.AddListener(PlayButtonClickSound);
    
    AddVisualHoverEffects(level1Button);
    AddVisualHoverEffects(level2Button);
}

void AddVisualHoverEffects(Button button)
{
    ColorBlock colors = button.colors;
    colors.normalColor = Color.white;
    colors.highlightedColor = new Color(0.9f, 0.9f, 1.0f); 
    colors.pressedColor = new Color(0.7f, 0.7f, 1.0f); 
    colors.selectedColor = new Color(0.9f, 0.9f, 1.0f);
    button.colors = colors;
    
}
    
    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position);
        }
    }
    
    void PlayButtonHoverSound()
    {
        if (buttonHoverSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonHoverSound, Camera.main.transform.position, 0.5f); 
        }
    }
    
    void InitializeHighScores()
    {
        level1HighScoreText.text = "0";
        level1BestTimeText.text = "00:00:00";
        level2HighScoreText.text = "0"; 
        level2BestTimeText.text = "00:00:00";
        
    }
    
    void LoadHighScores()
    {
    }
    
    string FormatTime(float timeInSeconds)
    {
        int hours = (int)(timeInSeconds / 3600);
        int minutes = (int)((timeInSeconds % 3600) / 60);
        int seconds = (int)(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
