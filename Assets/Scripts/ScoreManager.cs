using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    private BoardManager board;

    public TextMeshProUGUI scoreText;
    public int score;

    public Image scoreBar;

    void Start()
    {
        board = Object.FindAnyObjectByType<BoardManager>();

        UpdateScoreText();
        UpdateBar();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;

        UpdateScoreText();
        UpdateBar();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void UpdateBar()
    {
        if (board == null || scoreBar == null)
        {
            return;
        }

        if (board.scoreGoals == null || board.scoreGoals.Length == 0)
        {
            return;
        }

        int goal = board.scoreGoals[0];

        if (goal <= 0)
        {
            return;
        }

        scoreBar.fillAmount = Mathf.Clamp01((float)score / goal);
    }
}