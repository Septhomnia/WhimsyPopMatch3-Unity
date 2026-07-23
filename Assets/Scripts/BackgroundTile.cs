using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints = 1;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();

        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
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