using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Deck : MonoBehaviour, IOnClickDownUIElement
{
    public List<Card> playerDeck = new List<Card>();

    [Button] public void SendDeckData()
    {
        WebSocketService.SetDeck(GetDeckJSON());
    }


    public void OnClickElement()
    {
        if(GameManager.Instance.playerStats.playerHandCards < GameManager.Instance.maxHandSize)
        {
            WebSocketService.DrawCard();
            GameManager.Instance.playerStats.playerHandCards++;
            GameManager.Instance.PlayerDrawCard();
        }
        else
        {
            Debug.LogWarning("Hand full display error effect here");
        }
        
    }

    public string GetDeckJSON()
    {
        DeckObject deck = new DeckObject(playerDeck);
        return JsonUtility.ToJson(deck);
    }
}
