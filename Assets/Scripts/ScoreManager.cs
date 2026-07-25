using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private int score = 0;

    private void Start()
    {
        UpdateScoreText();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}