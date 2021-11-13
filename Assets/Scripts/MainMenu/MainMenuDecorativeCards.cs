using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDecorativeCards : MonoBehaviour
{
    private List<CardList.ListCard> allCards;
    void Start()
    {
        CardList cardList = Resources.Load("Card List") as CardList;
        allCards = cardList.allCards;

        foreach(Transform child in transform)
        {
            Card randomCard = allCards[Random.Range(0, (allCards.Count - 1))].card;
            DecorativeCard cardScript = child.GetComponent<DecorativeCard>();
            cardScript.card = randomCard;
            cardScript.UpdateUI();
        }
    }
}
