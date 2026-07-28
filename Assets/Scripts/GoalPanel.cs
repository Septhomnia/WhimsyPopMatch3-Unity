using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalPanel : MonoBehaviour
{
    public Image thisImage;
    public TextMeshProUGUI thisText;

    public Sprite thisSprite;
    public string thisString;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        if (thisImage != null)
        {
            thisImage.sprite = thisSprite;
        }

        if (thisText != null)
        {
            thisText.text = thisString;
        }
    }
}