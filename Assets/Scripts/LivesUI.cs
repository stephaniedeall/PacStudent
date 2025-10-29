using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Image livesImage;              
    public Sprite[] livesSprites;         
    public int currentLives = 3;           

    void Start()
    {
        UpdateLivesUI();
    }

    public void LoseLife()
    {
        currentLives = Mathf.Max(0, currentLives - 1);
        UpdateLivesUI();
    }

    public void GainLife()
    {
        currentLives = Mathf.Min(3, currentLives + 1);
        UpdateLivesUI();
    }

    void UpdateLivesUI()
    {
        livesImage.sprite = livesSprites[currentLives];
    }
}

