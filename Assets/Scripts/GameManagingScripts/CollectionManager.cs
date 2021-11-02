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
    [SerializeField] private GameObject setActiveButton;
    [SerializeField] private GameObject cardListPrefab;
    [SerializeField] private Toggle cardListTogglePrefab;
    public int activeList = 0;
    [SerializeField] private int lastActiveDeck = 0;
    [SerializeField] private int playerDeckLimit = 5;
    private Color32 defaultDeckBGColor = new Color32(89, 89, 89, 255);
    private Color32 activeDeckBGColor = new Color32(40, 205, 40 ,255);




    void Start()
    {
        resourcesCardList = Resources.Load("Card List") as CardList;

        // Test decks for dev purposes
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
        
        // Calling functions that create and populate the ingame lists
        SetAllCardsList();
        for (int i = 0; playerDecks.Count > i; i++)
        {
            CreateNewDeck("DECK " + (i + 1), true);
            SetPlayerDeckList(i);
        }
        UpdatePageText();

        createButton.GetComponent<Button>().onClick.AddListener(() => CreateButtonCallback("", false));
        setActiveButton.GetComponent<Button>().onClick.AddListener(() => SetActiveButtonCallback());
    }


    private void SetPlayerDecks(List<List<Card>> decks)
    {

    }

    // Sends the players active deck data to the online database and visually shows which deck is active
    public void SetActiveDeck()
    {
        if (activeList == 0) return;

        DeckObject deckString = new DeckObject(playerDecks[activeList - 1]);
        WebSocketService.SetDeck(JsonUtility.ToJson(deckString));
        SetActiveDeckUI();
    }

    // Sets the visuals that show which deck is active
    public void SetActiveDeckUI()
    {
        if (lastActiveDeck != 0)
        {
            cardListToggles[lastActiveDeck].transform.Find("Background").GetComponent<Image>().color = defaultDeckBGColor;
        };
        cardListToggles[activeList].transform.Find("Background").GetComponent<Image>().color = activeDeckBGColor;
        lastActiveDeck = activeList;
    }

    // Gets all game cards and passes them to the 'All Cards List' and calls it's populate function
    private void SetAllCardsList()
    {
        foreach (CardList.ListCard listCard in resourcesCardList.allCards)
        {
            cardLists[0].GetComponent<CollectionCardList>().cards.Add(listCard.card);
        }
        cardLists[0].GetComponent<CollectionCardList>().PopulatePage(1);
    }

    // Passes a specific player deck's cards to the corresponding ingame list and calls it's populate function
    public void SetPlayerDeckList(int i)
    {
        cardLists[i + 1].GetComponent<CollectionCardList>().cards = playerDecks[i];
        cardLists[i + 1].GetComponent<CollectionCardList>().PopulatePage(1);

    }

    // Creates new empty ingame list and deck when called
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

        // Force rebuilds the toggle row so that the content size fitter component behaves properly
        RectTransform togglesRectTransform = togglesRow.transform.parent.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(togglesRectTransform);
    }

    // Check's which toggle is checked and sets the corresponding ingame list active
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

    // Changes the page on the active ingame list
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

    // Updates the page counter
    public void UpdatePageText()
    {
        CollectionCardList list = cardLists[activeList].GetComponent<CollectionCardList>();
        pageText.GetComponent<TextMeshProUGUI>().text = (list.currentPage) + "/" + list.totalPages;
    }

    // Bound to an onClick event from the "Create Button'. Calls CreateNewDeck function
    private void CreateButtonCallback(string name, bool start)
    {
        CreateNewDeck(name, start);
    }

    // Bound to an onClick event from the "Set Active Button'. Calls SetActiveDeck function
    private void SetActiveButtonCallback()
    {
        SetActiveDeck();
    }
}
