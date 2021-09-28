using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardInHand : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement
{
    public bool mouseOverElement = false;
    private UiCardPreviewManager uiCardPreviewManager;
    private Canvas uiCanvas;
    private Canvas canvas;
    private GameObject cardPreview;

    private void Awake()
    {
        uiCardPreviewManager = GameObject.Find("CardHighlighter").GetComponent<UiCardPreviewManager>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
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

        Vector2 uiCanvasDimensions = uiCanvas.GetComponent<RectTransform>().sizeDelta;

        Vector2 pos = (Vector2)Camera.main.WorldToScreenPoint(GetComponent<Transform>().position);

        Vector2 canvasDimensions = canvas.GetComponent<RectTransform>().sizeDelta;

        Vector2 rel = new Vector2(pos.x / canvasDimensions.x, pos.y/ canvasDimensions.y);
        Vector2 posInUiCanvas = new Vector2(rel.x * uiCanvasDimensions.x, rel.y * uiCanvasDimensions.y) - uiCanvasDimensions/2;

        cardPreview = uiCardPreviewManager.ShowCardPreview(posInUiCanvas);
    }
    public void ScaleCardDown()
    {
        transform.parent.parent.GetComponent<UiHand>().DecreaseCardSize(transform.parent.gameObject);
        uiCardPreviewManager.HideCardPreview(cardPreview);
    }

    public void OnHoverEnter()
    {
        Debug.Log("Mouse over");
        if (!mouseOverElement)
        {
            ScaleCardUp();
            mouseOverElement = true;
        }
    }

    public void OnHoverExit()
    {
        if (mouseOverElement)
        {
            ScaleCardDown();
            mouseOverElement = false;
        }
    }
}
