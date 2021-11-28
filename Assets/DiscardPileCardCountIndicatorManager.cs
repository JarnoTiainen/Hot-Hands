using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscardPileCardCountIndicatorManager : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] private TextMeshProUGUI discardPileCardCountText;
    [SerializeField] private int owner;

    public void OnHoverEnter()
    {
        if(GameManager.Instance.IsYou(owner))
        {
            discardPileCardCountText.text = GameManager.Instance.playerStats.discardpileCardCount.ToString();
        }
        else
        {
            discardPileCardCountText.text = GameManager.Instance.enemyPlayerStats.discardpileCardCount.ToString();
        }
    }

    public void OnHoverExit()
    {
        discardPileCardCountText.text = "";
    }
}
