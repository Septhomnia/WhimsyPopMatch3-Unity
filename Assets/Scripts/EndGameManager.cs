using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject timeLabel;
    public TMP_Text counter;

    public GameObject tryAgainPanel;
    public GameObject youWinPanel;

    public EndGameRequirements requirements;

    public int currentCounterValue;

    private float timerSeconds = 1f;
    private BoardManager board;
    private FadePanelController fadePanel;

    void Start()
    {
        board = Object.FindAnyObjectByType<BoardManager>();
        fadePanel = Object.FindAnyObjectByType<FadePanelController>();

        SetupGame();
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;

        if (requirements.gameType == GameType.Moves)
        {
            if (movesLabel != null)
            {
                movesLabel.SetActive(true);
            }

            if (timeLabel != null)
            {
                timeLabel.SetActive(false);
            }
        }
        else
        {
            if (movesLabel != null)
            {
                movesLabel.SetActive(false);
            }

            if (timeLabel != null)
            {
                timeLabel.SetActive(true);
            }
        }

        if (counter != null)
        {
            counter.text = "" + currentCounterValue;
        }

        if (tryAgainPanel != null)
        {
            tryAgainPanel.SetActive(false);
        }

        if (youWinPanel != null)
        {
            youWinPanel.SetActive(false);
        }
    }

    public void DecreaseCounterValue()
    {
        if (board == null)
        {
            return;
        }

        if (board.currentState == GameState.pause ||
            board.currentState == GameState.win ||
            board.currentState == GameState.lose)
        {
            return;
        }

        currentCounterValue--;

        if (currentCounterValue <= 0)
        {
            currentCounterValue = 0;
        }

        if (counter != null)
        {
            counter.text = "" + currentCounterValue;
        }

        if (currentCounterValue <= 0)
        {
            LoseGame();
        }
    }

    void Update()
    {
        if (board == null)
        {
            return;
        }

        if (requirements.gameType == GameType.Time &&
            board.currentState == GameState.move &&
            currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;

            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1f;
            }
        }
    }

    public void WinGame()
    {
        if (youWinPanel != null)
        {
            youWinPanel.SetActive(true);
        }

        if (board != null)
        {
            board.currentState = GameState.win;
        }

        currentCounterValue = 0;

        if (counter != null)
        {
            counter.text = "" + currentCounterValue;
        }

        Debug.Log("You Win!");
    }

    public void LoseGame()
    {
        if (tryAgainPanel != null)
        {
            tryAgainPanel.SetActive(true);
        }

        if (board != null)
        {
            board.currentState = GameState.lose;
        }

        if (fadePanel != null)
        {
            fadePanel.GameOver();
        }

        currentCounterValue = 0;

        if (counter != null)
        {
            counter.text = "" + currentCounterValue;
        }

        Debug.Log("You Lose!");
    }
}