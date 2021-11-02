using UnityEngine;

public class HandCard : MonoBehaviour, IOnClickDownUIElement
{
    public void OnClickElement()
    {
        if(!Mouse.Instance.targetModeOn)
        {
            if (!GetComponent<InGameCard>().cardHidden)
            {
                transform.parent.GetComponent<Hand>().RemoveVisibleCard(gameObject);
                Mouse.Instance.SetNewHeldCard(gameObject, Hand.Instance.GetCardIndex(gameObject));
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            Debug.Log("targeting card " + gameObject.GetComponent<InGameCard>().cardData.cardName);
        }
    }
}
