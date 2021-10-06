using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Deck : MonoBehaviour, IOnClickDownUIElement
{
    public List<Card> playerDeck = new List<Card>();


    void Awake()
    {
        
        
    }

    private void Start()
    {
        
    }

    [Button] public void SendDeckData()
    {
        WebSocketService.SetDeck(GetDeckJSON());
    }


    public void OnClickElement()
    {
        WebSocketService.DrawCard();
        Hand.AddNewCard();
        gameObject.GetComponent<DrawCardSounds>().Draw();
    }


    public string GetDeckJSON()
    {
        DeckObject deck = new DeckObject(playerDeck);
        Debug.Log(JsonUtility.ToJson(deck));
        return JsonUtility.ToJson(deck);
    }
}
