using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckBuild : MonoBehaviour
{
    private CardList resourcesCardList;

    [SerializeField]
    private GameObject deckBuildCardPrefab;
    public List<BuildCard> deck = new List<BuildCard>();
    public int deckSizeLimit = 20;
    public int currentBuildSize = 0;
    [SerializeField]
    private GameObject countText;

    public GameObject settingsManager;
    public List<Card> playerDeck = new List<Card>();


    private void Start()
    {
        UpdateBuildSize();
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
                Debug.Log(deck[i].name);

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
        resourcesCardList = Resources.Load("Card List") as CardList;

        playerDeck.Clear();

        for (int i = 0; deck.Count > i; i++)
        {
            if (deck[i].amount == 1)
            {
                playerDeck.Add(deck[i].card);
            }
            else
            {
                for(int j = 0; deck[i].amount > j; j++)
                {
                    playerDeck.Add(deck[i].card);
                }
            }
        }

        resourcesCardList.playerDeck = playerDeck;
        settingsManager.GetComponent<CollectionManager>().SetCardLists(1);
    }

    public void ResetDeck()
    {
        deck.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        UpdateBuildSize();
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
