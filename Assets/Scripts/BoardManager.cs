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
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    public GameObject breakableTilePrefab;
    private BackgroundTile[,] breakableTiles;
    void Start()
    {
        blankSpaces = new bool[width, height];
        breakableTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];

        findMatches = Object.FindAnyObjectByType<FindMatches>();

        GenerateBlankSpaces();
        GenerateBreakableTiles();
        SetUp();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
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
                    Vector2 tilePosition = new Vector2(i, j);
                    Vector2 dotPosition = new Vector2(i, j + offSet);

                    GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                    tile.transform.parent = this.transform;
                    tile.name = "(" + i + "," + j + ")";

                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }

                    GameObject dot = Instantiate(dots[dotToUse], dotPosition, Quaternion.identity);
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
            Debug.Log("Matched dot destroyed at: " + column + "," + row);

            findMatches.currentMatches.Remove(allDots[column, row]);

            if (breakableTiles[column, row] != null)
            {
                Debug.Log("Breakable damaged at: " + column + "," + row);
                breakableTiles[column, row].TakeDamage(1);
            }
            else
            {
                Debug.Log("No breakable tile at: " + column + "," + row);
            }

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
    private void DebugCreateBreakableTile()
    {
        if (breakableTilePrefab == null)
        {
            Debug.LogError("Breakable Tile Prefab is EMPTY in BoardManager Inspector!");
            return;
        }

        Vector2 testPosition = new Vector2(3, 3);

        GameObject tile = Instantiate(
            breakableTilePrefab,
            testPosition,
            Quaternion.identity
        );

        tile.transform.parent = this.transform;
        tile.name = "DEBUG Breakable Tile";

        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("DEBUG Breakable Tile has NO SpriteRenderer!");
            return;
        }

        if (sr.sprite == null)
        {
            Debug.LogError("DEBUG Breakable Tile SpriteRenderer has NO sprite assigned!");
            return;
        }

        sr.sortingLayerName = "Default";

        Debug.Log("DEBUG breakable tile created at: " + testPosition);
    }
    private void CheckToMakeBombs()
    {
        int matchCount = findMatches.currentMatches.Count;

        Debug.Log("Bomb check match count: " + matchCount);

        if (matchCount == 4)
        {
            findMatches.CheckBombs();
        }
        else if (matchCount >= 5)
        {
            if (ColumnOrRow())
            {
                findMatches.CheckColorBombs();
            }
            else
            {
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
    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                int x = boardLayout[i].x;
                int y = boardLayout[i].y;

                Vector2 tempPosition = new Vector2(x, y);

                GameObject tile = Instantiate(
                    breakableTilePrefab,
                    tempPosition,
                    Quaternion.identity
                );

                tile.transform.parent = this.transform;
                tile.name = "Breakable Tile (" + x + "," + y + ")";

                BackgroundTile backgroundTile = tile.GetComponent<BackgroundTile>();

                if (backgroundTile == null)
                {
                    Debug.LogError("Breakable Tile prefabýnda BackgroundTile scripti yok: " + x + "," + y);
                }
                else
                {
                    breakableTiles[x, y] = backgroundTile;
                    Debug.Log("Breakable REGISTERED at: " + x + "," + y);
                }

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    sr.sortingOrder = -1;
                }
            }
        }
    }
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
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y];

        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];

        allDots[column, row] = holder;
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Horizontal check
                    if (i < width - 2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    // Vertical check
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }


        return false;
    }
    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);

        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }

        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Check right
                    if (i < width - 1 && allDots[i + 1, j] != null)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    // Check up
                    if (j < height - 1 && allDots[i, j + 1] != null)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
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
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();

        // Board üzerindeki bütün mevcut dotlarý listeye ekle
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                    allDots[i, j] = null;
                }
            }
        }

        // Board'daki boþ olmayan yerlere dotlarý rastgele geri yerleþtir
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIterations = 0;

                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }

                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();

                    piece.column = i;
                    piece.row = j;
                    piece.isMatched = false;
                    piece.otherDot = null;

                    allDots[i, j] = newBoard[pieceToUse];

                    newBoard.RemoveAt(pieceToUse);
                }
            }
        }

        if (IsDeadlocked())
        {
            Debug.Log("Still deadlocked, shuffling again.");
            ShuffleBoard();
        }
        else
        {
            Debug.Log("Board shuffled successfully.");
        }
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

        if (IsDeadlocked())
        {
            Debug.Log("Deadlocked!!!");
            ShuffleBoard();
        }

        currentState = GameState.move;
    }
}