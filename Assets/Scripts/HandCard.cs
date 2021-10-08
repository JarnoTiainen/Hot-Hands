using UnityEngine;

public class HandCard : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    public Mouse mouse;

    private void Awake()
    {
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
    }

    public void OnHoverEnter()
    {
        Hand.Instance.ShowCardTooltip(gameObject);
    }

    public void OnHoverExit()
    {
        Hand.Instance.HideCardTooltip(gameObject);
    }

    public void OnClickElement()
    {
        if(!GetComponent<InGameCard>().cardHidden)
        {
            transform.parent.GetComponent<Hand>().RemoveVisibleCard(gameObject);
            mouse.SetNewHeldCard(gameObject, Hand.Instance.GetCardIndex(gameObject));
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
