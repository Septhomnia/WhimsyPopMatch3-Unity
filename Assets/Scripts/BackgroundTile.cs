using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints = 1;

    private SpriteRenderer sprite;
    private GoalManager goalManager;

    private void Start()
    {
        goalManager = Object.FindAnyObjectByType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }

            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    private void MakeLighter()
    {
        if (sprite == null)
        {
            return;
        }

        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}