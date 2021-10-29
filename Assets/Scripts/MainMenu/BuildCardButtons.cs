using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCardButtons : MonoBehaviour
{
    public Card card;

    public void AddCard()
    {
        gameObject.transform.parent.GetComponent<DeckBuilder>().AddCard(card);
    }

    public void DeleteCard()
    {
        gameObject.transform.parent.GetComponent<DeckBuilder>().DeleteCard(card);
    }
}
