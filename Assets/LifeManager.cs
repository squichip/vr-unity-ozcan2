using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance;

    public int maxLives = 3;
    private int currentLives;

    public TextMeshProUGUI lifeText;
    public GameObject losePanel; // "Kaybettiniz" yazısı

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateUI();
        losePanel.SetActive(false);
    }

    public void LoseLife()
    {
        currentLives--;
        UpdateUI();

        if (currentLives < 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        lifeText.text = currentLives.ToString();
    }

    void GameOver()
    {
        losePanel.SetActive(true);
        Invoke(nameof(ReturnToMenu), 2f);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // MENÜ SAHNE ADI
    }
}
