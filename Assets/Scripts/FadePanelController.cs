using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoAnim;

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

        StartCoroutine(GameStartCo());
    }

    public void GameOver()
    {
        if (panelAnim != null)
        {
            panelAnim.SetBool("Out", false);
            panelAnim.SetBool("Game Over", true);
        }
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);

        BoardManager board = Object.FindAnyObjectByType<BoardManager>();

        if (board != null)
        {
            board.currentState = GameState.move;
        }
    }
}