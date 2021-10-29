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
    public List<BuildCard> deck = new List<BuildCard>();
    public List<Card> playerDeck = new List<Card>();
    [SerializeField] private GameObject deckBuildCardPrefab;
    public int deckSizeLimit = 20;
    public int currentBuildSize = 0;
    [SerializeField] public GameObject saveButton;
    [SerializeField] public GameObject copyButton;
    [SerializeField] public GameObject clearButton;



    private void Start()
    {
        cm = settingsManager.GetComponent<CollectionManager>();
        UpdateBuildSize();
    }

    private void OnEnable()
    {
        saveButton.GetComponent<Button>().onClick.AddListener(() => saveButtonCallback());
        copyButton.GetComponent<Button>().onClick.AddListener(() => CopyButtonCallback());
        clearButton.GetComponent<Button>().onClick.AddListener(() => clearButtonCallback());

    }

    public void AddCard(Card card)
    {
        if(currentBuildSize >= deckSizeLimit)
        {
            return;
        }

        if (deck.Count == 0)
        {
            AddNewCard(card);
            return;
        }

        for (int i = 0; deck.Count > i; i++)
        {

            if (deck[i].name == card.name)
            {
                deck[i].amount++;
                gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().amount++;
                gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().UpdateAmount();
                UpdateBuildSize();
                return;
            }
        }
        AddNewCard(card);
    }

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
        deck.Add(buildCard);
        UpdateBuildSize();
    }

    public void DeleteCard(Card card)
    {
        if (currentBuildSize <= 0)
        {
            return;
        }

        for (int i = 0; deck.Count > i; i++)
        {
            if (deck[i].name == card.name)
            {
                if(deck[i].amount == 1)
                {
                    deck.RemoveAt(i);
                    Destroy(GameObject.Find(card.name));
                }
                else
                {
                    deck[i].amount--;
                    gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().amount--;
                    gameObject.transform.Find(card.name).GetComponent<BuildCardUI>().UpdateAmount();
                }
                UpdateBuildSize();
                return;
            }
        }
    }

    private void UpdateBuildSize()
    {
        int count = 0;
        for(int i = 0; deck.Count > i; i++)
        {
            count += deck[i].amount;
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

    public void SaveDeck()
    {
        if (cm.activeList == 0) return;
        int playerDeckIndex = cm.activeList -1;
        List<Card> tempDeck = new List<Card>();
        for (int i = 0; deck.Count > i; i++)
        {
            if (deck[i].amount == 1)
            {
                tempDeck.Add(deck[i].card);
            }
            else
            {
                for(int j = 0; deck[i].amount > j; j++)
                {
                    tempDeck.Add(deck[i].card);
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
        ClearBuild();
        cm.UpdatePageText();
    }

    public void CopyDeck()
    {
        if (cm.activeList == 0) return;
        int playerDeckIndex = cm.activeList - 1;
        foreach(Card card in cm.playerDecks[playerDeckIndex])
        {
            AddCard(card);
        }
    }

    public void ClearBuild()
    {
        deck.Clear();
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
