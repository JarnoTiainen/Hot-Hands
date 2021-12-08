using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentCountManager : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] private InGameCard inGameCard;


    public void OnHoverEnter()
    {
        if(inGameCard.GetData().enchantments.Count > 0)
        {
            Tooltip.ShowTooltip_Static(inGameCard.GetData().description);
        }
    }

    public void OnHoverExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
