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

    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    public GoalPanel[] currentGoals;

    void Start()
    {
        SetupGoals();
    }

    void SetupGoals()
    {
        foreach (Transform child in goalIntroParent.transform)
        {
            Destroy(child.gameObject);
        }

        currentGoals = new GoalPanel[levelGoals.Length];

        Debug.Log("Goal count: " + levelGoals.Length);

        for (int i = 0; i < levelGoals.Length; i++)
        {
            Debug.Log("Creating goal: " + i);

            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform);
            goal.transform.localScale = Vector3.one;

            GoalPanel panel = goal.GetComponent<GoalPanel>();

            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
            panel.SetUp();

            currentGoals[i] = panel;
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
            currentGoals[i].thisText.text =
                levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;

                currentGoals[i].thisText.text =
                    levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }

        if (goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("You win!");
        }
    }
}