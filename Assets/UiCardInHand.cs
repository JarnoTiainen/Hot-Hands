using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardInHand : MonoBehaviour
{
    public bool mouseOverElement = false;
    private UiCardPreviewManager uiCardPreviewManager;

    private void Awake()
    {
        uiCardPreviewManager = GameObject.Find("CardHighlighter").GetComponent<UiCardPreviewManager>();
    }

    public void OnMouseOver()
    {
        Debug.Log("Mouse over");
        if (!mouseOverElement) {
            ScaleCardUp();
            mouseOverElement = true;
        }
    }

    public void OnMouseExit()
    {
        if (mouseOverElement)
        {
            ScaleCardDown();
            mouseOverElement = false;
        }
    }

    public void OnMouseDown()
    {
        GrabCard();
    }

    public void GrabCard()
    {

    }

    public void ScaleCardUp()
    {
        transform.parent.parent.GetComponent<UiHand>().IncreaseCardSize(transform.parent.gameObject);
        uiCardPreviewManager.ShowCardPreview(Camera.main.WorldToScreenPoint(GetComponent<RectTransform>().localPosition));
    }
    public void ScaleCardDown()
    {
        transform.parent.parent.GetComponent<UiHand>().DecreaseCardSize(transform.parent.gameObject);
    }
}
