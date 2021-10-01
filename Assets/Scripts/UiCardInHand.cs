using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardInHand : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement, IOnClickUpElement
{
    public CardData cardData;

    public bool mouseOverElement = false;
    public Mouse mouse;

    private void Awake()
    {
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
    }

    public void OnMouseDown()
    {
        GrabCard();
    }

    public void GrabCard()
    {

    }

    public void OnHoverEnter()
    {
        if (!mouseOverElement)
        {
            Hand.Instance.ShowCardTooltip(transform.parent.gameObject);
            mouseOverElement = true;
        }
    }

    public void OnHoverExit()
    {
        Debug.Log("Hover exit");
        if (mouseOverElement)
        {
            Hand.Instance.HideCardTooltip(transform.parent.gameObject);
            mouseOverElement = false;
        }
    }

    public void OnClickElement()
    {
        transform.parent.parent.GetComponent<Hand>().RemoveVisibleCard(transform.parent.gameObject);
        mouse.SetNewHeldCard(transform.parent.gameObject, Hand.Instance.GetCardIndex(transform.parent.gameObject));
        gameObject.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("Clicked element");
    }

    public void OnClickUpElement()
    {
        Debug.Log("OnClick up");
    }
}
