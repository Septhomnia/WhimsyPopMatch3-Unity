using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoAnim;

    private BoardManager board;

    private void Start()
    {
        board = Object.FindAnyObjectByType<BoardManager>();

        if (board != null)
        {
            board.currentState = GameState.wait;
        }
    }

    public void OK()
    {
        if (panelAnim != null)
        {
            panelAnim.SetBool("Out", true);
        }

        if (gameInfoAnim != null)
        {
            gameInfoAnim.SetBool("Out", true);
        }

        if (board != null)
        {
            board.currentState = GameState.move;
        }
    }
}