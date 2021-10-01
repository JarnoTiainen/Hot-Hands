using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardInHand : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement, IOnClickUpElement
{
    public CardData cardData;

    public bool mouseOverElement = false;
    private UiCardPreviewManager uiCardPreviewManager;
    private Canvas uiCanvas;
    private Canvas canvas;
    private GameObject cardPreview;
    public Mouse mouse;

    private void Awake()
    {
        uiCardPreviewManager = GameObject.Find("CardHighlighter").GetComponent<UiCardPreviewManager>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
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
        Vector2 uiCanvasDimensions = uiCanvas.GetComponent<RectTransform>().sizeDelta;

        Vector2 pos = (Vector2)Camera.main.WorldToScreenPoint(GetComponent<Transform>().position);

        Vector2 canvasDimensions = canvas.GetComponent<RectTransform>().sizeDelta;

        Vector2 rel = new Vector2(pos.x / canvasDimensions.x, pos.y/ canvasDimensions.y);
        Vector2 posInUiCanvas = new Vector2(rel.x * uiCanvasDimensions.x, rel.y * uiCanvasDimensions.y) - uiCanvasDimensions/2;

        cardPreview = uiCardPreviewManager.ShowCardPreview(posInUiCanvas);
    }
    public void ScaleCardDown()
    {
        uiCardPreviewManager.HideCardPreview(cardPreview);
    }

    public void OnHoverEnter()
    {
        if (!mouseOverElement)
        {
            UiHand.Instance.ShowCardTooltip(transform.parent.gameObject);
            mouseOverElement = true;
        }
    }

    public void OnHoverExit()
    {
        Debug.Log("Hover exit");
        if (mouseOverElement)
        {
            UiHand.Instance.HideCardTooltip(transform.parent.gameObject);
            mouseOverElement = false;
        }
    }

    public void OnClickElement()
    {
        transform.parent.parent.GetComponent<UiHand>().RemoveVisibleCard(transform.parent.gameObject);
        mouse.SetNewHeldCard(transform.parent.gameObject, UiHand.Instance.GetCardIndex(transform.parent.gameObject));
        gameObject.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("Clicked element");
    }

    public void OnClickUpElement()
    {
        Debug.Log("OnClick up");
    }
}
