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

    public List<Card> deck = new List<Card>();
    public List<List<Card>> playerDecks = new List<List<Card>>();

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

    public Card GetCardData(string cardName)
    {
        Card card = null;
        foreach (CardList.ListCard listCard in allCards)
        {
            if (listCard.name == cardName)
            {
                card = listCard.card;
            }
        }
        if (card != null)
        {
            return card;
        }
        else
        {
            Debug.LogError("Card with name: " + cardName + " was not found from Unity side databse");
            return null;
        }
    }

    public CardData GetCardData(SummonCardMessage summonCardMessage)
    {
        Card card = GetCardData(summonCardMessage.cardName);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(summonCardMessage.enchantments), summonCardMessage.cardName, summonCardMessage.cardCost, summonCardMessage.cardValue, card.cardType, card.attackDirection, summonCardMessage.rp, summonCardMessage.lp, summonCardMessage.enchantments, summonCardMessage.seed, GetCardDescription(summonCardMessage.enchantments), summonCardMessage.legendary);

        return cardData;
    }

    public CardData GetCardData(PlayCardMessage playCardMessage)
    {
        Card card = GetCardData(playCardMessage.cardName);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(playCardMessage.enchantments), playCardMessage.cardName, playCardMessage.cardCost, playCardMessage.cardValue, card.cardType, card.attackDirection, playCardMessage.rp, playCardMessage.lp, playCardMessage.enchantments, playCardMessage.seed, GetCardDescription(playCardMessage.enchantments), playCardMessage.legendary);

        return cardData;
    }

    public CardData GetCardData(DrawCardMessage drawCardMessage)
    {
        Card card = GetCardData(drawCardMessage.cardName);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(drawCardMessage.enchantments), drawCardMessage.cardName, drawCardMessage.cardCost, drawCardMessage.cardValue, card.cardType, (Card.AttackDirection)drawCardMessage.attackDirection, drawCardMessage.rp, drawCardMessage.lp, drawCardMessage.enchantments, drawCardMessage.seed, GetCardDescription(drawCardMessage.enchantments), drawCardMessage.legendary);
        //Debug.Log("new card seed: " + cardData.seed);
        return cardData;
    }

    public CardData GetCardData(PlaySpellMessage playSpellMessage)
    {
        Debug.Log("GEtting card data with name: " + playSpellMessage.cardName);
        Card card = GetCardData(playSpellMessage.cardName);
        Debug.Log("Card image name is: " + card.cardSprite.name);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(playSpellMessage.enchantments), playSpellMessage.cardName, playSpellMessage.cardCost, playSpellMessage.cardValue, card.cardType, playSpellMessage.enchantments, playSpellMessage.seed, GetCardDescription(playSpellMessage.enchantments), playSpellMessage.legendary);
        return cardData;
    }

    public string GetCardDescription(List<Enchantment> enchantments)
    {
        string effect = "";
        foreach(Enchantment enchantment in enchantments)
        {
            effect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
        }
        return effect;
    }

    public bool CheckIfCardIsTargetting(List<Enchantment> enchantments)
    {
        foreach(Enchantment enchantment in enchantments)
        {
            if (enchantment.targeting) return true;
        }
        return false;
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
