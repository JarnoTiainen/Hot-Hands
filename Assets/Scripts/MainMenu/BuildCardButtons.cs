using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildCardButtons : MonoBehaviour
{
    public Card card;
    bool colorset = false;

    [SerializeField] private Image image;
    [SerializeField] private Color spellColor;

    public void AddCard()
    {
        gameObject.transform.parent.GetComponent<DeckBuilder>().AddCard(card);

        
        
        
    }

    private void Update()
    {
        if(card != null)
        {
            if (card.cardType == Card.CardType.Spell)
            {
                image.color = spellColor;
            }
        }
    }

    public void DeleteCard()
    {
        gameObject.transform.parent.GetComponent<DeckBuilder>().DeleteCard(card);
    }
}
