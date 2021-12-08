using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DeckBuilder : MonoBehaviour
{
    public static DeckBuilder Instance { get; private set; }
    private CollectionManager cm;
    private List<BuildCard> build = new List<BuildCard>();
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI legendaryCountText;
    [SerializeField] private GameObject deckBuildCardPrefab;
    public int deckSizeLimit = 20;
    [SerializeField] private int duplicateLimit = 2;
    [SerializeField] private int legendaryLimit = 2;
    private int currentBuildSize = 0;
    private int legendaryAmount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cm = CollectionManager.Instance;
        UpdateBuildSize();
    }

    // Adds a card to the builder
    public void AddCard(Card card)
    {
        // Reached max size
        if (currentBuildSize >= deckSizeLimit)
        {
            MainMenu.Instance.CreatePopupNotification("Can not have more than " + deckSizeLimit + " cards in deck!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
            return;
        }
        // Add duplicate card to build
        for (int i = 0; build.Count > i; i++)
        {
            if (build[i].card.cardName == card.cardName)
            {
                if (build[i].legendary)
                {
                    MainMenu.Instance.CreatePopupNotification("Can not have duplicates of legendary cards!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
                    return;
                }
                if(build[i].amount == duplicateLimit)
                {
                    MainMenu.Instance.CreatePopupNotification("Can not have more than " + duplicateLimit + " duplicates of same card!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
                    return;
                }
                BuildCardScript buildCardScript = gameObject.transform.Find(card.name).GetComponent<BuildCardScript>();
                if(buildCardScript.amount == (duplicateLimit - 1)) buildCardScript.AddButtonSetActive(false);

                build[i].amount++;
                buildCardScript.amount++;
                buildCardScript.UpdateAmount();
                UpdateBuildSize();
                return;
            }
        }

        // Add non-duplicate card to build
        AddNewCard(card);
    }

    // Adds a non-duplicate card to the builder
    private void AddNewCard(Card card)
    {
        if (card.legendary && legendaryAmount >= legendaryLimit)
        {
            MainMenu.Instance.CreatePopupNotification("Can not have more than " + legendaryLimit + " legendary cards in one deck!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
            return;
        }
        if (card.legendary) legendaryAmount++;

        GameObject buildCardGameObject = Instantiate(deckBuildCardPrefab);
        BuildCardScript buildCardScript = buildCardGameObject.GetComponent<BuildCardScript>();
        buildCardGameObject.SetActive(true);
        buildCardGameObject.name = card.name;
        buildCardScript.card = card;
        buildCardScript.amount = 1;
        buildCardScript.Initialize();
        buildCardGameObject.transform.SetParent(gameObject.transform, false);

        BuildCard buildCard = new BuildCard(card);
        buildCard.amount = 1;
        build.Add(buildCard);
        UpdateBuildSize();
    }

    // Deletes a card from the builder
    public void DeleteCard(Card card)
    {
        // Build has 0 cards
        if (currentBuildSize <= 0)
        {
            return;
        }
        // Build has > 0 cards
        for (int i = 0; build.Count > i; i++)
        {
            if (build[i].card.cardName == card.cardName)
            {
                // Card being deleted is the only one of it's type in the build
                if(build[i].amount == 1)
                {
                    if (build[i].legendary) legendaryAmount--;
                    build.RemoveAt(i);
                    Destroy(transform.Find(card.name).gameObject);
                }
                // Card being deleted has duplicates in the build
                else
                {
                    BuildCardScript buildCardScript = gameObject.transform.Find(card.name).GetComponent<BuildCardScript>();
                    build[i].amount--;
                    buildCardScript.amount--;
                    buildCardScript.UpdateAmount();
                    buildCardScript.AddButtonSetActive(true);
                }
                UpdateBuildSize();
            }
        }
    }

    // Updates the builder's card counter text
    private void UpdateBuildSize()
    {
        int count = 0;
        for(int i = 0; build.Count > i; i++)
        {
            count += build[i].amount;
        }

        currentBuildSize = count;
        if(currentBuildSize < 10)
        {
            countText.text = "0" + currentBuildSize + "/" + deckSizeLimit;
        }
        else
        {
            countText.text = currentBuildSize + "/" + deckSizeLimit;
        }
        legendaryCountText.text = legendaryAmount + "/" + legendaryLimit;
    }

    // Open selected deck
    [Button]public void OpenDeck(int index)
    {
        ClearBuild();
        foreach (Card card in cm.playerDecks[index])
        {
            AddCard(card);
        }
    }

    // Saves the build on DeckBuilder's list on to the currently open deck and sets it as the player's active deck
    public void SaveDeck()
    {
        int deckIndex = -1;
        for (int i = 0; cm.deckToggles.Count > i; i++)
        {
            if (cm.deckToggles[i].deckSelected)
            {
                deckIndex = i;
                break;
            }
        }
        if (deckIndex == -1) return;

        List<Card> tempDeck = new List<Card>();
        for (int i = 0; build.Count > i; i++)
        {
            // Only one card of it's type in the build
            if (build[i].amount == 1)
            {
                tempDeck.Add(build[i].card);
            }
            // Build has duplicates of the card
            else
            {
                for(int j = 0; build[i].amount > j; j++)
                {
                    tempDeck.Add(build[i].card);
                }
            }
        }
        // Make sure deck is full
        if(tempDeck.Count < deckSizeLimit)
        {
            MainMenu.Instance.CreatePopupNotification("Deck has to have " + deckSizeLimit + " cards!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
            return;
        }
        // Sorts alphabetically
        tempDeck.Sort(delegate(Card card1, Card card2) 
        {
            return card1.cardName.CompareTo(card2.cardName);
        });
        // Add deck to CollectionManager's playerDecks
        if (cm.playerDecks[deckIndex] == null)
        {
            cm.playerDecks.Add(tempDeck);
        }
        else
        {
            cm.playerDecks[deckIndex] = tempDeck;
        }
        cm.SaveDeckToDB(deckIndex);
    }

    // Clears all the cards from the builder
    public void ClearBuild()
    {
        legendaryAmount = 0;
        build.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        UpdateBuildSize();
    }

    private class BuildCard
    {
        public string name;
        public Card card;
        public int amount;
        public bool legendary;

        public BuildCard(Card card)
        {
            this.name = card.cardName;
            this.card = card;
            this.legendary = card.legendary;
        }
    }
}
