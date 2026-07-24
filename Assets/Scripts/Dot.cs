using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    public GameObject otherDot;
    private BoardManager board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;


    public GameObject rowArrow;
    public GameObject columnArrow;
    private GameObject arrow;
    public GameObject colorBomb;
    private GameObject colorBombObject;
    public GameObject adjacentMarker;
    private GameObject adjacentMarkerObject;
    private HintManager hintManager;
    void Start()
    {

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;


        board = Object.FindFirstObjectByType<BoardManager>();
        findMatches = Object.FindFirstObjectByType<FindMatches>();
        hintManager = Object.FindAnyObjectByType<HintManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        previousRow = row;
        previousColumn = column;
    }


    //This is for testing and Debug only.
    private void OnMouseOver()
    {
        // DEBUG ONLY: Right click makes this dot a color bomb.
        if (Input.GetMouseButtonDown(1))
        {
            MakeAdjacentBomb();
        }
    }
    void Update()
    {
        /*
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        Color currentColor = mySprite.color;
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }
        */
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target

            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            //findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            //Directly set the position
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            //findMatches.FindAllMatches();


        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }
    public void MakeColorBomb()
    {
        isColorBomb = true;
        isRowBomb = false;
        isColumnBomb = false;
        isAdjacentBomb = false;

        if (arrow != null)
        {
            Destroy(arrow);
        }

        if (colorBombObject != null)
        {
            Destroy(colorBombObject);
        }

        colorBombObject = Instantiate(colorBomb);
        colorBombObject.transform.SetParent(transform, false);
        colorBombObject.transform.localPosition = Vector3.zero;
        colorBombObject.transform.localRotation = Quaternion.identity;
        colorBombObject.transform.localScale = Vector3.one;
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            Dot otherDotScript = otherDot.GetComponent<Dot>();

            // Color bomb check
            if (isColorBomb)
            {
                findMatches.MatchPiecesOfColor(otherDot.tag);
                isMatched = true;
                board.DestroyMatches();
            }
            else if (otherDotScript.isColorBomb)
            {
                findMatches.MatchPiecesOfColor(this.gameObject.tag);
                otherDotScript.isMatched = true;
                board.DestroyMatches();
            }
            else
            {
                // Normal match check
                findMatches.FindAllMatches();

                yield return new WaitForSeconds(.3f);

                if (!isMatched && !otherDotScript.isMatched)
                {
                    // Invalid move, swap back
                    otherDotScript.row = row;
                    otherDotScript.column = column;

                    row = previousRow;
                    column = previousColumn;

                    yield return new WaitForSeconds(.5f);

                    board.currentState = GameState.move;
                    board.currentDot = null;
                }
                else
                {
                    // Valid move, destroy matched dots
                    board.DestroyMatches();
                }
            }
        }
        else
        {
            board.currentState = GameState.move;
            board.currentDot = null;
        }
    }
    private void OnMouseDown()
    {
        if (hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //Debug.Log(firstTouchPosition);
    }
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = (int)(float)(Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI);
            // Debug.Log(swipeAngle);
            board.currentState = GameState.wait;
            board.currentDot = this;
            MovePieces();
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
            board.currentDot = null;
        }
    }
    void FindMatches()
    {
        // Horizontal check: left - current - right
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];

            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        // Vertical check: up - current - down
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];

            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
    private void OnDestroy()
    {
        if (adjacentMarkerObject != null)
        {
            Destroy(adjacentMarkerObject);
        }
    }
    public void MakeRowBomb()
    {
        isRowBomb = true;
        isColumnBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        if (arrow != null)
        {
            Destroy(arrow);
        }

        if (colorBombObject != null)
        {
            Destroy(colorBombObject);
        }

        if (adjacentMarkerObject != null)
        {
            Destroy(adjacentMarkerObject);
        }

        arrow = Instantiate(rowArrow);
        arrow.transform.SetParent(transform, false);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;
        arrow.transform.localScale = Vector3.one;
    }
    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];

        previousRow = row;
        previousColumn = column;

        if (otherDot != null)
        {
            otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
            otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;

            column += (int)direction.x;
            row += (int)direction.y;

            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
            board.currentDot = null;
        }
    }
    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        if (arrow != null)
        {
            Destroy(arrow);
        }

        if (colorBombObject != null)
        {
            Destroy(colorBombObject);
        }

        if (adjacentMarkerObject != null)
        {
            Destroy(adjacentMarkerObject);
        }

        arrow = Instantiate(columnArrow);
        arrow.transform.SetParent(transform, false);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;
        arrow.transform.localScale = Vector3.one;
    }
    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        isRowBomb = false;
        isColumnBomb = false;
        isColorBomb = false;

        if (arrow != null)
        {
            Destroy(arrow);
        }

        if (colorBombObject != null)
        {
            Destroy(colorBombObject);
        }

        if (adjacentMarkerObject != null)
        {
            Destroy(adjacentMarkerObject);
        }

        if (adjacentMarker == null)
        {
            Debug.LogError("Adjacent Marker prefab is not assigned on " + gameObject.name);
            return;
        }

        adjacentMarkerObject = Instantiate(adjacentMarker);
        adjacentMarkerObject.transform.SetParent(transform, false);
        adjacentMarkerObject.transform.localPosition = Vector3.zero;
        adjacentMarkerObject.transform.localRotation = Quaternion.identity;
        adjacentMarkerObject.transform.localScale = Vector3.one;

        SpriteRenderer markerSprite = adjacentMarkerObject.GetComponent<SpriteRenderer>();
        SpriteRenderer dotSprite = GetComponent<SpriteRenderer>();

        if (markerSprite != null && dotSprite != null)
        {
            markerSprite.sortingLayerID = dotSprite.sortingLayerID;
            markerSprite.sortingOrder = dotSprite.sortingOrder + 1;
        }
    }
}