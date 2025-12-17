using UnityEngine;
using TMPro;

public class SimpleScoreManager : MonoBehaviour
{
    public int Score { get; private set; }
    public TextMeshProUGUI scoreText;

    private void Start() => Refresh();

    public void Add(int amount)
    {
        Score += amount;
        Refresh();
    }

    void Refresh()
    {
        if (scoreText != null)
            scoreText.text = "SKOR: " + Score;
    }
}
