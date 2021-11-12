using UnityEngine;

public class HandCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] private bool onTargetMode;
    [SerializeField] private GameObject twirlEffect;

    public void OnClickElement()
    {
        if(!Mouse.Instance.targetModeOn)
        {
            if (!GetComponent<InGameCard>().cardHidden)
            {
                transform.parent.GetComponent<Hand>().RemoveVisibleCard(gameObject);
                transform.localPosition = Vector3.zero;
                Mouse.Instance.SetNewHeldCard(gameObject);
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            Debug.Log("targeting card " + gameObject.GetComponent<InGameCard>().cardData.cardName);
        }
    }

    public void StartPuffEffect()
    {
       
    }

    public void SwitchToTargetMode()
    {
        if(!onTargetMode)
        {
            GetComponent<InGameCard>().SpellBurn();
            GameObject newTwirlEffect = Instantiate(twirlEffect);
            newTwirlEffect.transform.SetParent(transform);
            newTwirlEffect.transform.localPosition = Vector3.zero;
            onTargetMode = true;
        }
    }
}
