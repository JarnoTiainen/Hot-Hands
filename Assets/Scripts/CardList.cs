using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "CardList")]
public class CardList : ScriptableObject
{
    public List<ListCard> allCards = new List<ListCard>();
    public List<Card> cardList = new List<Card>();

    public void AddAddCardToList(Card card)
    {
        foreach(ListCard listCard in allCards)
        {
            if (listCard.name == card.cardName)
            {
                listCard.card = card;
                Debug.Log("Updated " + card.cardName);
                return;
            }
        }
        Debug.Log("Added " + card.cardName + " to list");
        allCards.Add(new ListCard(card));
    }

    [Button]public void UpdateAllCards()
    {
        List<ListCard> deletableCards = new List<ListCard>();
        foreach (ListCard listCard in allCards)
        {
            if (listCard.card) 
            {
                WebSocketService.SaveCardToDataBase(listCard.card);
            }
            else deletableCards.Add(listCard);

        }
        foreach (ListCard card in deletableCards) allCards.Remove(card);
    }

    [Button]public void PrintAllCards()
    {
        Debug.Log("Total card: " + allCards.Count);
        foreach (ListCard listCard in allCards)
        {
            Debug.Log("name: " +listCard.name);
        }
    }


    [Serializable]
    public class ListCard
    {
        public string name;
        public Card card;

        public ListCard(Card card)
        {
            this.name = card.cardName;
            this.card = card;
        }
    }
}
