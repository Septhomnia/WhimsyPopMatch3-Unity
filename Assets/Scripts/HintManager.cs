using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private BoardManager board;

    public float hintDelay = 5f;
    private float hintDelaySeconds;

    public GameObject hintParticle;
    public GameObject currentHint;

    void Start()
    {
        board = Object.FindAnyObjectByType<BoardManager>();
        hintDelaySeconds = hintDelay;
    }

    void Update()
    {
        if (board == null)
        {
            return;
        }

        if (board.currentState != GameState.move)
        {
            return;
        }

        hintDelaySeconds -= Time.deltaTime;

        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    private List<GameObject> FindPossibleMoves()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            if (!possibleMoves.Contains(board.allDots[i, j]))
                            {
                                possibleMoves.Add(board.allDots[i, j]);
                            }
                        }
                    }

                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            if (!possibleMoves.Contains(board.allDots[i, j]))
                            {
                                possibleMoves.Add(board.allDots[i, j]);
                            }
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    private GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = FindPossibleMoves();

        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }

        return null;
    }

    private void MarkHint()
    {
        GameObject move = PickOneRandomly();

        if (move != null && hintParticle != null)
        {
            currentHint = Instantiate(
                hintParticle,
                move.transform.position,
                Quaternion.identity
            );

            currentHint.transform.parent = this.transform;
        }
    }

    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
        }

        hintDelaySeconds = hintDelay;
    }
}