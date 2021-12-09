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
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(summonCardMessage.enchantments), summonCardMessage.cardName, summonCardMessage.cardCost, summonCardMessage.cardValue, card.cardType, card.attackDirection, summonCardMessage.rp, summonCardMessage.lp, summonCardMessage.enchantments, summonCardMessage.seed, summonCardMessage.legendary, CheckCardTargetType(summonCardMessage.enchantments));
        cardData.description = GetCardDescription(cardData);
        return cardData;
    }

    public CardData GetCardData(PlayCardMessage playCardMessage)
    {
        Card card = GetCardData(playCardMessage.cardName);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(playCardMessage.enchantments), playCardMessage.cardName, playCardMessage.cardCost, playCardMessage.cardValue, card.cardType, card.attackDirection, playCardMessage.rp, playCardMessage.lp, playCardMessage.enchantments, playCardMessage.seed, playCardMessage.legendary, CheckCardTargetType(playCardMessage.enchantments));
        cardData.description = GetCardDescription(cardData);
        return cardData;
    }

    public CardData GetCardData(DrawCardMessage drawCardMessage)
    {
        Card card = GetCardData(drawCardMessage.cardName);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(drawCardMessage.enchantments), drawCardMessage.cardName, drawCardMessage.cardCost, drawCardMessage.cardValue, card.cardType, (Card.AttackDirection)drawCardMessage.attackDirection, drawCardMessage.rp, drawCardMessage.lp, drawCardMessage.enchantments, drawCardMessage.seed, drawCardMessage.legendary, CheckCardTargetType(drawCardMessage.enchantments));
        cardData.description = GetCardDescription(cardData);
        //Debug.Log("new card seed: " + cardData.seed);
        return cardData;
    }

    public CardData GetCardData(PlaySpellMessage playSpellMessage)
    {
        Debug.Log("GEtting card data with name: " + playSpellMessage.cardName);
        Card card = GetCardData(playSpellMessage.cardName);
        Debug.Log("Card image name is: " + card.cardSprite.name);
        CardData cardData = new CardData(card.cardSprite, CheckIfCardIsTargetting(playSpellMessage.enchantments), playSpellMessage.cardName, playSpellMessage.cardCost, playSpellMessage.cardValue, card.cardType, playSpellMessage.enchantments, playSpellMessage.seed, playSpellMessage.legendary);
        cardData.description = GetCardDescription(cardData);
        return cardData;
    }

    public string GetCardDescription(CardData cardData, bool hiddenDescriptions = false)
    {
        string effect = "";
        string openerEffects = "";
        string wildEffects = "";
        string lastBreathEffect = "";
        string sacrificeEffect = "";
        string spellEffect = "";
        foreach(Enchantment enchantment in cardData.enchantments)
        {
            if(!enchantment.hidden || hiddenDescriptions)
            {
                switch (enchantment.trigger)
                {
                    case Enchantment.Trigger.Opener:
                        if (openerEffects != "") openerEffects += " and ";
                        openerEffects += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.LastBreath:
                        if (lastBreathEffect != "") lastBreathEffect += " and ";
                        lastBreathEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Drawtivation:
                        if (wildEffects != "") wildEffects += " and ";
                        wildEffects += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Sacrifice:
                        if (sacrificeEffect != "") sacrificeEffect += " and ";
                        sacrificeEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Spell:
                        if (spellEffect != "") spellEffect += " and ";
                        spellEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                }
            }
            
            
        }
        if (spellEffect != "") effect += (spellEffect);
        if (wildEffects != "")
        {
            effect += ("<b>Wild</b>: " + wildEffects + "\n");
        }
        if (sacrificeEffect != "")
        {
            effect += ("<b>Sacrifice</b>: " + sacrificeEffect + "\n");
        }
        if (openerEffects != "")
        {
            effect += ("<b>Opener</b>: " + openerEffects + "\n");
        }
        if (lastBreathEffect != "")
        {
            effect += ("<b>Lastbreath</b>: " + lastBreathEffect + "\n");
        }
        return effect;
    }

    public string GetCardDescription(List<Enchantment> enchantments)
    {
        string effect = "";
        string openerEffects = "";
        string wildEffects = "";
        string lastBreathEffect = "";
        string sacrificeEffect = "";
        string spellEffect = "";
        foreach (Enchantment enchantment in enchantments)
        {
            if (!enchantment.hidden)
            {
                switch (enchantment.trigger)
                {
                    case Enchantment.Trigger.Opener:
                        if (openerEffects != "") openerEffects += " and ";
                        openerEffects += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.LastBreath:
                        if (lastBreathEffect != "") lastBreathEffect += " and ";
                        lastBreathEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Drawtivation:
                        if (wildEffects != "") wildEffects += " and ";
                        wildEffects += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Sacrifice:
                        if (sacrificeEffect != "") sacrificeEffect += " and ";
                        sacrificeEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                    case Enchantment.Trigger.Spell:
                        if (spellEffect != "") spellEffect += " and ";
                        spellEffect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
                        break;
                }
            }


        }
        if (spellEffect != "") effect += (spellEffect);
        if (wildEffects != "")
        {
            effect += ("<b>Wild</b>: " + wildEffects + "\n");
        }
        if (sacrificeEffect != "")
        {
            effect += ("<b>Sacrifice</b>: " + sacrificeEffect + "\n");
        }
        if (openerEffects != "")
        {
            effect += ("<b>Opener</b>: " + openerEffects + "\n");
        }
        if (lastBreathEffect != "")
        {
            effect += ("<b>Lastbreath</b>: " + lastBreathEffect + "\n");
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

    public Enchantment.TargetType CheckCardTargetType(List<Enchantment> enchantments)
    {
        foreach (Enchantment enchantment in enchantments)
        {
            if (enchantment.targeting)
            {
                return enchantment.targetType;
            }
        }
        return Enchantment.TargetType.Both;
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
