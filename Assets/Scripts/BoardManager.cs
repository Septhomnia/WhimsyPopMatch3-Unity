using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Tile[,] allTiles;
    void Start()
    {
        allTiles = new Tile[width, height];
        SetUp();
    }
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject Tile = Instantiate(tilePrefab, tempPosition ,Quaternion.identity) as GameObject;
                Tile.transform.parent = this.transform;
                Tile.name = "(" + i + "," + j + ")";

            }
        }

    }
}