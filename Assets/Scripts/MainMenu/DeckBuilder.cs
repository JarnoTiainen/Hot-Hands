using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour
{
    private CollectionManager cm;
    [SerializeField] private GameObject countText;
    public GameObject settingsManager;
    public List<BuildCard> build = new List<BuildCard>();
    [SerializeField] private GameObject deckBuildCardPrefab;
    public int deckSizeLimit = 20;
    public int currentBuildSize = 0;
    [SerializeField] public GameObject saveButton;
    [SerializeField] public GameObject copyButton;
    [SerializeField] public GameObject clearButton;



    private void Start()
    {
        cm = settingsManager.GetComponent<CollectionManager>();
        saveButton.GetComponent<Button>().onClick.AddListener(() => saveButtonCallback());
        copyButton.GetComponent<Button>().onClick.AddListener(() => CopyButtonCallback());
        clearButton.GetComponent<Button>().onClick.AddListener(() => clearButtonCallback());
        UpdateBuildSize();
    }


    // Adds a card to the builder
    public void AddCard(Card card)
    {
        if(currentBuildSize >= deckSizeLimit)
        {
            return;
        }

        if (build.Count == 0)
        {
            AddNewCard(card);
            return;
        }

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
        AddNewCard(card);
    }

    // Adds a non-duplicate card to the builder
    private void AddNewCard(Card card)
    {
        GameObject buildCardGameObject = Instantiate(deckBuildCardPrefab) as GameObject;
        buildCardGameObject.SetActive(true);
        buildCardGameObject.name = card.name;
        buildCardGameObject.GetComponent<BuildCardUI>().cardName = card.name;
        buildCardGameObject.GetComponent<BuildCardUI>().amount = 1;
        buildCardGameObject.GetComponent<BuildCardButtons>().card = card;
        buildCardGameObject.GetComponent<BuildCardUI>().UpdateName();
        buildCardGameObject.GetComponent<BuildCardUI>().UpdateAmount();
        buildCardGameObject.transform.SetParent(gameObject.transform, false);

        BuildCard buildCard = new BuildCard(card);
        buildCard.amount = 1;
        build.Add(buildCard);
        UpdateBuildSize();
    }

    // Deletes a card from the builder
    public void DeleteCard(Card card)
    {
        if (currentBuildSize <= 0)
        {
            return;
        }

        for (int i = 0; build.Count > i; i++)
        {
            if (build[i].name == card.name)
            {
                if(build[i].amount == 1)
                {
                    build.RemoveAt(i);
                    Destroy(GameObject.Find(card.name));
                }
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
        int playerDeckIndex = cm.activeList -1;
        List<Card> tempDeck = new List<Card>();
        for (int i = 0; build.Count > i; i++)
        {
            if (build[i].amount == 1)
            {
                tempDeck.Add(build[i].card);
            }
            else
            {
                for(int j = 0; build[i].amount > j; j++)
                {
                    tempDeck.Add(build[i].card);
                }
            }
        }
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

    public class BuildCard
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
