using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildCardScript : MonoBehaviour
{
    public int amount;
    public Card card;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject addButton;
    [SerializeField] private Color spellColor;
    [SerializeField] private Color legendaryColor;
    [SerializeField] private Image image;
    private DeckBuilder deckBuilder;

    private void Start()
    {
        deckBuilder = DeckBuilder.Instance;
    }

    public void Initialize()
    {
        nameText.text = card.cardName;
        amountText.text = "x" + amount.ToString();
        if (card.cardType == Card.CardType.Spell)
        {
            image.color = spellColor;
        }
        if (card.legendary)
        {
            image.color = legendaryColor;
            addButton.SetActive(false);
        }
    }

    public void AddButtonSetActive(bool value) => addButton.SetActive(value);

    public void UpdateAmount()
    {
        amountText.text = "x" + amount.ToString();
    }

    public void AddCard() => deckBuilder.AddCard(card);

    public void DeleteCard() => deckBuilder.DeleteCard(card);
}
