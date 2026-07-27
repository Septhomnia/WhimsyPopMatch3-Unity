using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private BoardManager board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = Object.FindFirstObjectByType<BoardManager>();
    }

    public void FindAllMatches()
    {
        currentMatches.Clear();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    // Horizontal check
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            Dot currentDotScript = currentDot.GetComponent<Dot>();
                            Dot leftDotScript = leftDot.GetComponent<Dot>();
                            Dot rightDotScript = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                AddPiecesToMatches(IsRowBomb(currentDotScript, leftDotScript, rightDotScript));
                                AddPiecesToMatches(IsColumnBomb(currentDotScript, leftDotScript, rightDotScript));
                                AddPiecesToMatches(IsAdjacentBomb(currentDotScript, leftDotScript, rightDotScript));

                                AddToMatchList(leftDot);
                                AddToMatchList(rightDot);
                                AddToMatchList(currentDot);
                            }
                        }
                    }

                    // Vertical check
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dot currentDotScript = currentDot.GetComponent<Dot>();
                            Dot upDotScript = upDot.GetComponent<Dot>();
                            Dot downDotScript = downDot.GetComponent<Dot>();

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                AddPiecesToMatches(IsRowBomb(currentDotScript, upDotScript, downDotScript));
                                AddPiecesToMatches(IsColumnBomb(currentDotScript, upDotScript, downDotScript));
                                AddPiecesToMatches(IsAdjacentBomb(currentDotScript, upDotScript, downDotScript));

                                AddToMatchList(upDot);
                                AddToMatchList(downDot);
                                AddToMatchList(currentDot);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("FindAllMatches finished. Count: " + currentMatches.Count);
    }


    private void AddToMatchList(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }

        dot.GetComponent<Dot>().isMatched = true;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb)
        {
            currentDots.AddRange(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            currentDots.AddRange(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currentDots.AddRange(GetRowPieces(dot3.row));
        }

        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
        {
            currentDots.AddRange(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentDots.AddRange(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currentDots.AddRange(GetColumnPieces(dot3.column));
        }

        return currentDots;
    }
    private List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                    }
                }
            }
        }

        return dots;
    }
    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb)
        {
            currentDots.AddRange(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentDots.AddRange(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentDots.AddRange(GetAdjacentPieces(dot3.column, dot3.row));
        }

        return currentDots;
    }
    private void AddPiecesToMatches(List<GameObject> pieces)
    {
        foreach (GameObject piece in pieces)
        {
            AddToMatchList(piece);
        }
    }
    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color)
                    {
                        AddToMatchList(board.allDots[i, j]);
                    }
                }
            }
        }
    }
    private List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                GameObject currentPiece = board.allDots[column, i];
                Dot dot = currentPiece.GetComponent<Dot>();

                AddPieceToList(dots, currentPiece);

                if (dot.isRowBomb)
                {
                    for (int j = 0; j < board.width; j++)
                    {
                        AddPieceToList(dots, board.allDots[j, i]);
                    }
                }
            }
        }

        return dots;
    }
    private void AddPieceToList(List<GameObject> list, GameObject piece)
    {
        if (piece != null && !list.Contains(piece))
        {
            list.Add(piece);
        }
    }
    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                GameObject currentPiece = board.allDots[i, row];
                Dot dot = currentPiece.GetComponent<Dot>();

                AddPieceToList(dots, currentPiece);

                if (dot.isColumnBomb)
                {
                    for (int j = 0; j < board.height; j++)
                    {
                        AddPieceToList(dots, board.allDots[i, j]);
                    }
                }
            }
        }

        return dots;
    }
    public void CheckColorBombs()
    {
        Dot dotToMakeBomb = GetBombPiece();

        if (dotToMakeBomb == null)
        {
            return;
        }

        if (!dotToMakeBomb.isColorBomb)
        {
            dotToMakeBomb.isMatched = false;
            dotToMakeBomb.MakeColorBomb();
        }
    }
    public void CheckAdjacentBombs()
    {
        Dot dotToMakeBomb = GetBombPiece();

        if (dotToMakeBomb == null)
        {
            return;
        }

        if (!dotToMakeBomb.isAdjacentBomb)
        {
            dotToMakeBomb.isMatched = false;
            dotToMakeBomb.MakeAdjacentBomb();
        }
    }
    private Dot GetBombPiece()
    {
        if (board.currentDot == null)
        {
            return null;
        }

        if (board.currentDot.isMatched)
        {
            return board.currentDot;
        }

        if (board.currentDot.otherDot != null)
        {
            Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();

            if (otherDot.isMatched)
            {
                return otherDot;
            }
        }

        return null;
    }
    public void CheckBombs()
    {
        Dot dotToMakeBomb = GetBombPiece();

        if (dotToMakeBomb == null)
        {
            return;
        }

        dotToMakeBomb.isMatched = false;

        int typeOfBomb = Random.Range(0, 100);

        if (typeOfBomb < 50)
        {
            dotToMakeBomb.MakeRowBomb();
        }
        else
        {
            dotToMakeBomb.MakeColumnBomb();
        }
    }
}