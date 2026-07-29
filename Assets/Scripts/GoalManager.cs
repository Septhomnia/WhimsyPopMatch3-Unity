using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;

    public GameObject introGoalPrefab;
    public GameObject gameGoalPrefab;

    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    public GoalPanel[] currentGoals;

    private EndGameManager endGameManager;

    void Start()
    {
        endGameManager = Object.FindAnyObjectByType<EndGameManager>();
        SetupGoals();
    }

    void SetupGoals()
    {
        currentGoals = new GoalPanel[levelGoals.Length];

        // Clean intro goals
        if (goalIntroParent != null)
        {
            foreach (Transform child in goalIntroParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Clean game goals
        if (goalGameParent != null)
        {
            foreach (Transform child in goalGameParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < levelGoals.Length; i++)
        {
            // Goal shown on the intro panel
            if (goalIntroParent != null && introGoalPrefab != null)
            {
                GameObject introGoal = Instantiate(introGoalPrefab, goalIntroParent.transform);
                introGoal.transform.localScale = Vector3.one;

                GoalPanel introPanel = introGoal.GetComponent<GoalPanel>();

                if (introPanel != null)
                {
                    introPanel.thisSprite = levelGoals[i].goalSprite;
                    introPanel.thisString = "0/" + levelGoals[i].numberNeeded;
                    introPanel.SetUp();
                }
            }

            // Goal shown during gameplay
            if (goalGameParent != null && gameGoalPrefab != null)
            {
                GameObject gameGoal = Instantiate(gameGoalPrefab, goalGameParent.transform);
                gameGoal.transform.localScale = Vector3.one;

                GoalPanel gamePanel = gameGoal.GetComponent<GoalPanel>();

                if (gamePanel != null)
                {
                    gamePanel.thisSprite = levelGoals[i].goalSprite;
                    gamePanel.thisString = "0/" + levelGoals[i].numberNeeded;
                    gamePanel.SetUp();

                    currentGoals[i] = gamePanel;
                }
            }
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;

        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (currentGoals[i] != null && currentGoals[i].thisText != null)
            {
                currentGoals[i].thisText.text =
                    levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            }

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;

                if (currentGoals[i] != null && currentGoals[i].thisText != null)
                {
                    currentGoals[i].thisText.text =
                        levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
                }
            }
        }

        if (goalsCompleted >= levelGoals.Length)
        {
            if (endGameManager != null)
            {
                endGameManager.WinGame();
            }
        }
    }
}