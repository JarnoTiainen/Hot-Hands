using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour
{
    private CollectionManager cm;
    private List<BuildCard> build = new List<BuildCard>();
    [SerializeField] private GameObject countText;
    [SerializeField] private GameObject deckBuildCardPrefab;
    [SerializeField] private int deckSizeLimit = 20;
    private int currentBuildSize = 0;
    [SerializeField] private GameObject saveButton;
    [SerializeField] private GameObject copyButton;
    [SerializeField] private GameObject clearButton;



    private void Start()
    {
        cm = CollectionManager.Instance;
        saveButton.GetComponent<Button>().onClick.AddListener(() => saveButtonCallback());
        copyButton.GetComponent<Button>().onClick.AddListener(() => CopyButtonCallback());
        clearButton.GetComponent<Button>().onClick.AddListener(() => clearButtonCallback());
        UpdateBuildSize();
    }


    // Adds a card to the builder
    public void AddCard(Card card)
    {
        // Reached max size
        if(currentBuildSize >= deckSizeLimit)
        {
            return;
        }

        // First card in the build
        if (build.Count == 0)
        {
            AddNewCard(card);
            return;
        }

        // Add duplicate card to build
        for (int i = 0; build.Count > i; i++)
        {

            if (build[i].name == card.name)
            {
                build[i].amount++;
                gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().amount++;
                gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().UpdateAmount();
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
        GameObject buildCardGameObject = Instantiate(deckBuildCardPrefab) as GameObject;
        BuildCardUI buildCardGameObjectUI = buildCardGameObject.GetComponent<BuildCardUI>();
        buildCardGameObject.SetActive(true);
        buildCardGameObject.name = card.name;
        buildCardGameObjectUI.cardName = card.name;
        buildCardGameObjectUI.amount = 1;
        buildCardGameObjectUI.UpdateName();
        buildCardGameObjectUI.UpdateAmount();
        buildCardGameObject.GetComponent<BuildCardButtons>().card = card;
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
            if (build[i].name == card.name)
            {
                // Card being deleted is the only one of it's type in the build
                if(build[i].amount == 1)
                {
                    build.RemoveAt(i);
                    Destroy(GameObject.Find(card.name));
                }
                // Card being deleted has duplicates in the build
                else
                {
                    build[i].amount--;
                    gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().amount--;
                    gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().UpdateAmount();
                }
                UpdateBuildSize();
                return;
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
            countText.GetComponent<TextMeshProUGUI>().text = "0" + currentBuildSize + "/" + deckSizeLimit;
        }
        else
        {
            countText.GetComponent<TextMeshProUGUI>().text = currentBuildSize + "/" + deckSizeLimit;
        }

    }

    // Saves the build on DeckBuilder's list on to the currently open deck and sets it as the player's active deck
    public void SaveDeck()
    {
        if (cm.activeList == 0) return;

        int playerDeckIndex = cm.activeList - 1;
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
        // Sorts alphabetically
        tempDeck.Sort(delegate(Card card1, Card card2) 
        {
            return card1.cardName.CompareTo(card2.cardName);
        
        });
     
        if (cm.playerDecks[playerDeckIndex] == null)
        {
            cm.playerDecks.Add(tempDeck);
        }
        else
        {
            cm.playerDecks[playerDeckIndex] = tempDeck;
        }
        cm.SetPlayerDeckList(playerDeckIndex);
        cm.UpdatePageText();
        cm.SetActiveDeck();
        ClearBuild();
    }

    // Copies the cards from the currently open deck to the builder
    public void CopyDeck()
    {
        if (cm.activeList == 0) return;
        int playerDeckIndex = cm.activeList - 1;
        foreach(Card card in cm.playerDecks[playerDeckIndex])
        {
            AddCard(card);
        }
    }

    // Clears all the cards from the builder
    public void ClearBuild()
    {
        build.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        UpdateBuildSize();
    }

    private void CopyButtonCallback()
    {
        CopyDeck();
    }
    private void saveButtonCallback()
    {
        SaveDeck();
    }
    private void clearButtonCallback()
    {
        ClearBuild();
    }

    private class BuildCard
    {
        public string name;
        public Card card;
        public int amount;

        public BuildCard(Card card)
        {
            this.name = card.cardName;
            this.card = card;
        }
    }
}
