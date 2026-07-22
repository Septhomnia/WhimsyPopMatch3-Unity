using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState
{
    wait,
    move
}
public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class BoardManager : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    private Tile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    void Start()
    {
        allTiles = new Tile[width, height];
        allDots = new GameObject[width, height];
        findMatches = Object.FindAnyObjectByType<FindMatches>();
        blankSpaces = new bool[width, height];

        GenerateBlankSpaces();
        SetUp();

    }
    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);

                    GameObject tile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                    tile.transform.parent = this.transform;
                    tile.name = "(" + i + "," + j + ")";

                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag &&
                    allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        if (row > 1)
        {
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag &&
                    allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            findMatches.currentMatches.Remove(allDots[column, row]);
          
            GameObject effect = Instantiate(
                destroyEffect,
                allDots[column, row].transform.position,
                Quaternion.identity
            );

            Destroy(effect, 1f);

            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }
    private bool ColumnOrRow()
    {
        if (findMatches.currentMatches.Count == 0)
        {
            return false;
        }

        for (int row = 0; row < height; row++)
        {
            int numberHorizontal = 0;

            foreach (GameObject piece in findMatches.currentMatches)
            {
                if (piece != null)
                {
                    Dot dot = piece.GetComponent<Dot>();

                    if (dot.row == row)
                    {
                        numberHorizontal++;
                    }
                }
            }

            if (numberHorizontal >= 5)
            {
                return true;
            }
        }

        for (int column = 0; column < width; column++)
        {
            int numberVertical = 0;

            foreach (GameObject piece in findMatches.currentMatches)
            {
                if (piece != null)
                {
                    Dot dot = piece.GetComponent<Dot>();

                    if (dot.column == column)
                    {
                        numberVertical++;
                    }
                }
            }

            if (numberVertical >= 5)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckToMakeBombs()
    {
        int matchCount = findMatches.currentMatches.Count;

        Debug.Log("Bomb check match count: " + matchCount);

        if (matchCount == 4)
        {
            Debug.Log("Creating row/column bomb");
            findMatches.CheckBombs();
        }
        else if (matchCount >= 5)
        {
            if (ColumnOrRow())
            {
                Debug.Log("Creating color bomb");
                findMatches.CheckColorBombs();
            }
            else
            {
                Debug.Log("Creating adjacent bomb");
                findMatches.CheckAdjacentBombs();
            }
        }
    }
    public void DestroyMatches()
    {
        CheckToMakeBombs();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    //private IEnumerator DecreaseRowCo()
    //{
    //    int nullCount = 0;
    //    for (int i = 0; i < width; i++)
    //    {
    //        for (int j = 0; j < height; j++)
    //        {
    //            if (allDots[i, j] == null)
    //            {
    //                nullCount++;
    //            }
    //            else if (nullCount > 0)
    //            {
    //                allDots[i, j].GetComponent<Dot>().row -= nullCount;
    //                allDots[i, j] = null;
    //            }
    //        }
    //        nullCount = 0;
    //    }
    //    yield return new WaitForSeconds(.4f);
    //    StartCoroutine(FillBoardCo());
    //}
    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allDots[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j= 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for(int i = 0; i<width; i++)
        {
            for (int j = 0; j<height; j++)
            {
                if (allDots[i, j]!= null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;

    }
    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }


    }
        