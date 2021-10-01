using UnityEngine;

public class HandCard : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    public CardData cardData;
    public Mouse mouse;

    private void Awake()
    {
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
    }

    public void OnHoverEnter()
    {
        Hand.Instance.ShowCardTooltip(transform.parent.gameObject);
    }

    public void OnHoverExit()
    {
        Hand.Instance.HideCardTooltip(transform.parent.gameObject);
    }

    public void OnClickElement()
    {
        transform.parent.parent.GetComponent<Hand>().RemoveVisibleCard(transform.parent.gameObject);
        mouse.SetNewHeldCard(transform.parent.gameObject, Hand.Instance.GetCardIndex(transform.parent.gameObject));
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
