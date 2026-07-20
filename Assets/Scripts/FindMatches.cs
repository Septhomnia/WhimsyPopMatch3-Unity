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
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
       
        currentMatches.Clear();


        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    // Horizontal check: left - current - right
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                Dot currentDotScript = currentDot.GetComponent<Dot>();
                                Dot leftDotScript = leftDot.GetComponent<Dot>();
                                Dot rightDotScript = rightDot.GetComponent<Dot>();

                                // If one of the horizontal matched pieces is a row bomb,
                                // match the whole row.
                                if (currentDotScript.isRowBomb || leftDotScript.isRowBomb || rightDotScript.isRowBomb)
                                {
                                    AddPiecesToMatches(GetRowPieces(j));
                                }

                                // If one of the horizontal matched pieces is a column bomb,
                                // match that bomb's column.
                                if (currentDotScript.isColumnBomb)
                                {
                                    AddPiecesToMatches(GetColumnPieces(i));
                                }

                                if (leftDotScript.isColumnBomb)
                                {
                                    AddPiecesToMatches(GetColumnPieces(i - 1));
                                }

                                if (rightDotScript.isColumnBomb)
                                {
                                    AddPiecesToMatches(GetColumnPieces(i + 1));
                                }

                                AddToMatchList(leftDot);
                                AddToMatchList(rightDot);
                                AddToMatchList(currentDot);
                            }
                        }
                    }

                    // Vertical check: up - current - down
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                Dot currentDotScript = currentDot.GetComponent<Dot>();
                                Dot upDotScript = upDot.GetComponent<Dot>();
                                Dot downDotScript = downDot.GetComponent<Dot>();

                                // If one of the vertical matched pieces is a column bomb,
                                // match the whole column.
                                if (currentDotScript.isColumnBomb || upDotScript.isColumnBomb || downDotScript.isColumnBomb)
                                {
                                    AddPiecesToMatches(GetColumnPieces(i));
                                }

                                // If one of the vertical matched pieces is a row bomb,
                                // match that bomb's row.
                                if (currentDotScript.isRowBomb)
                                {
                                    AddPiecesToMatches(GetRowPieces(j));
                                }

                                if (upDotScript.isRowBomb)
                                {
                                    AddPiecesToMatches(GetRowPieces(j + 1));
                                }

                                if (downDotScript.isRowBomb)
                                {
                                    AddPiecesToMatches(GetRowPieces(j - 1));
                                }

                                AddToMatchList(upDot);
                                AddToMatchList(downDot);
                                AddToMatchList(currentDot);
                            }
                        }
                    }
                }
            }
        }
    }
   
    private void AddToMatchList(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }

        dot.GetComponent<Dot>().isMatched = true;
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
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j <board.height; j++)
            {
                //check if that piece exists
                if (board.allDots[i,j] != null)
                {
                    //check the tag on that dot
                    if (board.allDots[i,j].tag == color)
                    {
                        //set that dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
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
                dots.Add(board.allDots[column, i]);
            }
        }

        return dots;
    }

    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
            }
        }

        return dots;
    }
    
    public void CheckBombs()
    {
        // Did the player move something?
        if (board.currentDot != null)
        {
            // Is the piece they moved matched?
            if (board.currentDot.isMatched)
            {
                // Make it unmatched so it does not get destroyed.
                board.currentDot.isMatched = false;

                // Decide what kind of bomb to make.
                int typeOfBomb = Random.Range(0, 100);

                if (typeOfBomb < 50)
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            // Is the other piece matched?
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();

                if (otherDot.isMatched)
                {
                    // Make it unmatched so it does not get destroyed.
                    otherDot.isMatched = false;

                    // Decide what kind of bomb to make.
                    int typeOfBomb = Random.Range(0, 100);

                    if (typeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }

                }

            }
        }
    }
}