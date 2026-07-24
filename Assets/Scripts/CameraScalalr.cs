using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private BoardManager board;

    public float cameraOffset = -10f;
    public float aspectRatio = 0.625f;
    public float padding = 2f;
    public float yOffset = 1f;
    void Start()
    {
        board = Object.FindFirstObjectByType<BoardManager>();

        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2f, y / 2f + yOffset, cameraOffset);
        transform.position = tempPosition;

        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2f + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2f + padding;
        }
    }
}