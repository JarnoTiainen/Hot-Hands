using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CollectionManager : MonoBehaviour
{
    private CardList resourcesCardList;

    public List<Toggle> cardListToggles = new List<Toggle>();
    public List<GameObject> cardLists = new List<GameObject>();
    [SerializeField] private List<Card> testDeck = new List<Card>();
    [SerializeField] private List<Card> testDeck2 = new List<Card>();
    public List<List<Card>> playerDecks = new List<List<Card>>();
    [SerializeField] private GameObject cardListWindow;
    [SerializeField] private GameObject togglesRow;
    [SerializeField] private GameObject pageText;
    [SerializeField] private GameObject createButton;
    [SerializeField] private GameObject cardListPrefab;
    [SerializeField] private Toggle cardListTogglePrefab;
    public int activeList = 0;
    [SerializeField] private int activePlayerDeck = 0;
    [SerializeField] private int playerDeckLimit = 5;




    void Start()
    {
        resourcesCardList = Resources.Load("Card List") as CardList;
        SetAllCardsList();

        ////////////////////////////////////////////////////
        testDeck.Sort(delegate (Card card1, Card card2)
        {
            return card1.cardName.CompareTo(card2.cardName);

        });
        testDeck2.Sort(delegate (Card card1, Card card2)
        {
            return card1.cardName.CompareTo(card2.cardName);

        });

        playerDecks.Add(testDeck);
        playerDecks.Add(testDeck2);
        ////////////////////////////////////////////////////

        for (int i = 0; playerDecks.Count > i; i++)
        {
            CreateNewDeck("DECK " + (i + 1), true);
            SetPlayerDeckList(i);
        }
        UpdatePageText();
    }

    private void OnEnable()
    {
        togglesRow.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        togglesRow.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
        createButton.GetComponent<Button>().onClick.AddListener(() => createButtonCallback("", false));
        //Canvas.ForceUpdateCanvases();
    }

    private void SetPlayerDecks(List<List<Card>> decks)
    {

    }

    [Button]private void SetActiveDeck()
    {
        DeckObject deckString = new DeckObject(playerDecks[activeList]);
        WebSocketService.SetDeck(JsonUtility.ToJson(deckString));
    }

    private void SetAllCardsList()
    {
        foreach (CardList.ListCard listCard in resourcesCardList.allCards)
        {
            cardLists[0].GetComponent<CollectionCardList>().cards.Add(listCard.card);
        }
        cardLists[0].GetComponent<CollectionCardList>().PopulatePage(1);
    }

    public void SetPlayerDeckList(int i)
    {
        cardLists[i + 1].GetComponent<CollectionCardList>().cards = playerDecks[i];
        cardLists[i + 1].GetComponent<CollectionCardList>().PopulatePage(1);

    }

    public void CreateNewDeck(string newName = null, bool start = false)
    {
        if (playerDecks.Count >= playerDeckLimit) return;
        if (playerDeckLimit - playerDecks.Count == 1) createButton.SetActive(false);

        GameObject newCardList = Instantiate(cardListPrefab) as GameObject;
        newCardList.SetActive(false);
        if (newName == null || newName == "") newCardList.name = "DECK " + (playerDecks.Count + 1);
        else newCardList.name = newName;
        newCardList.transform.SetParent(cardListWindow.transform, false);
        cardLists.Add(newCardList);

        Toggle newToggle = Instantiate(cardListTogglePrefab) as Toggle;
        if (newName == null || newName == "") newToggle.name = "DECK " + (playerDecks.Count + 1);
        else newToggle.name = newName;
        newToggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = newCardList.name;
        newToggle.transform.SetParent(togglesRow.transform, false);
        newToggle.group = togglesRow.GetComponent<ToggleGroup>();
        cardListToggles.Add(newToggle);
        newToggle.GetComponent<CollectionToggle>().index = cardListToggles.IndexOf(newToggle);

        if (!start) playerDecks.Add(new List<Card>());
        
    }

    public void ChangeActiveCardList(int toggle)
    {
        if (!cardListToggles[toggle].isOn) return;

        for(int i = 0; cardListToggles.Count > i; i++)
        {
            if (cardListToggles[i].isOn)
            {
                cardLists[i].SetActive(true);
                activeList = toggle;
            }
            else
            {
                cardLists[i].SetActive(false);
            }
        }
        UpdatePageText();
    }

    public void ChangePage(int i)
    {
        CollectionCardList list = cardLists[activeList].GetComponent<CollectionCardList>();

        if(list.currentPage == 1 && i == -1)
        {
            return;
        }
        if(list.currentPage == list.totalPages && i == 1)
        {
            return;
        }
        list.PopulatePage(list.currentPage + i);
        UpdatePageText();
    }

    public void UpdatePageText()
    {
        CollectionCardList list = cardLists[activeList].GetComponent<CollectionCardList>();
        pageText.GetComponent<TextMeshProUGUI>().text = (list.currentPage) + "/" + list.totalPages;
    }

    private void createButtonCallback(string name, bool start)
    {
        CreateNewDeck(name, start);
    }
}
