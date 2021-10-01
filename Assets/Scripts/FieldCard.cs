using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class FieldCard : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement
{
    public void OnHoverEnter()
    {
        CardData cardData = gameObject.GetComponent<InGameCard>().GetCardData();
        Tooltip.ShowTooltip_Static(cardData.cardType.ToString(), cardData.cardName);
    }

    public void OnHoverExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
