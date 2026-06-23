using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
public class Dot : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    public int swipeAngle = 0;

    void Start()
    {

    }
    void Update()
    {
       
    }
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
    }
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
    void CalculateAngle()
    {
        swipeAngle = (int)(float)(Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI);
        Debug.Log(swipeAngle);
    }
}

    

