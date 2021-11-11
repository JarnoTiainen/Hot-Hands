using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Reflection;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }
    private CardList resourcesCardList;


    public List<Toggle> cardListToggles = new List<Toggle>();
    public List<GameObject> cardLists = new List<GameObject>();
    public List<List<Card>> playerDecks = new List<List<Card>>();
    public List<string> deckNames = new List<string>();
    [SerializeField] private GameObject cardListWindow;
    [SerializeField] private GameObject togglesRow;
    [SerializeField] private GameObject pageText;
    [SerializeField] private GameObject createButton;
    [SerializeField] private GameObject setActiveButton;
    [SerializeField] private GameObject cardListPrefab;
    [SerializeField] private Toggle cardListTogglePrefab;
    public int activeList = 0;
    private int lastActiveDeck = 0;
    public int playerDeckLimit = 5;
    private Color32 defaultDeckBGColor = new Color32(89, 89, 89, 255);
    private Color32 activeDeckBGColor = new Color32(40, 205, 40 ,255);


    void Start()
    {
        Instance = this;
        resourcesCardList = Resources.Load("Card List") as CardList;
        for(int i = 0; playerDeckLimit > i; i++)
        {
            deckNames.Add("");
        }

        SetAllCardsList();
        UpdatePageText();

        createButton.GetComponent<Button>().onClick.AddListener(() => CreateButtonCallback());
        setActiveButton.GetComponent<Button>().onClick.AddListener(() => SetActiveButtonCallback());
    }


    public void SetPlayerDecks(GetDecksMessage getDecksMessage)
    {
        ////////////////////////////
        //Debug.Log(getDecksMessage);
        //Debug.Log(getDecksMessage.decks.Count);
        //Debug.Log(getDecksMessage.decks[0].deck);
        //Debug.Log(getDecksMessage.decks[0].deck.Count);
        //Debug.Log(getDecksMessage.activeDeckIndex);
        //////////////////////////////

        List<GetDecksMessage.Deck> playerDecks = getDecksMessage.decks;
        List<string> deckNames = getDecksMessage.deckNames;

        foreach (GetDecksMessage.Deck deck in playerDecks)
        {
            List<Card> tempDeck = new List<Card>();

            foreach(string cardName in deck.deck)
            {
                for(int i = 0; resourcesCardList.allCards.Count > i; i++)
                {
                    if(cardName == resourcesCardList.allCards[i].name)
                    {
                        tempDeck.Add(resourcesCardList.allCards[i].card);
                        break;
                    }
                }
            }
            if(tempDeck.Count == 0)
            {
                this.playerDecks.Add(new List<Card>());
            }
            else
            {
                this.playerDecks.Add(tempDeck);
            }
        }

        for (int i = 0; playerDecks.Count > i; i++)
        {
            CreateNewDeck(deckNames[i], true, true);
            UpdateDeckUI(i);
        }

        lastActiveDeck = getDecksMessage.activeDeckIndex + 1;
        cardListToggles[getDecksMessage.activeDeckIndex + 1].transform.Find("Background").GetComponent<Image>().color = activeDeckBGColor;
        if (playerDecks.Count == playerDeckLimit)
        {
            createButton.SetActive(false);
        }
    }

    // Sends the players active deck index to the db and visually shows which deck is active
    public void SetActiveDeckToDB()
    {
        if (activeList == 0) return;
        WebSocketService.SetActiveDeck(activeList - 1);
        SetActiveDeckUI();
    }

    // Sends the players deck data to the db
    public void SaveDeckToDB(int index)
    {
        /*
         * @todo Needs to send deck name as well
         * @body :)))
         */
        DeckObject deckString = new DeckObject(playerDecks[index], index);
        WebSocketService.SaveDeck(JsonUtility.ToJson(deckString));
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
        // Add all cards in the resources allCards-list to Collection's "All Cards"
        CollectionCardList cardListScript = cardLists[0].GetComponent<CollectionCardList>();
        foreach (CardList.ListCard listCard in resourcesCardList.allCards)
        {
            cardListScript.cards.Add(listCard.card);
        }
        // Sort alphabetically
        cardListScript.SortList(CollectionCardList.SortMethod.Name);
        cardListScript.PopulatePage(1);
    }

    // Passes a specific player deck's cards to the corresponding ingame list and calls it's populate function
    public void UpdateDeckUI(int i)
    {
        // Updates cards on list
        CollectionCardList cardListScript = cardLists[i + 1].GetComponent<CollectionCardList>();
        cardListScript.cards = playerDecks[i];
        cardListScript.SortList(SortMethodDropdown.Instance.GetSortMethod(), SortMethodDropdown.Instance.reverse);

        // Updates name on toggle
        if(deckNames[i] == null || deckNames[i] == "")
        {
            cardListToggles[i + 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DECK " + (i + 1);
        }
        else
        {
            cardListToggles[i + 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = deckNames[i];
        }
    }

    // Creates new empty ingame list and deck when called
    public void CreateNewDeck(string newName = null, bool isStart = false, bool isLoadingFromDB = false)
    {
        if (playerDecks.Count >= playerDeckLimit && !isLoadingFromDB) return;
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
        newToggle.group = togglesRow.GetComponent<ToggleGroup>();
        newToggle.transform.SetParent(togglesRow.transform, false);
        cardListToggles.Add(newToggle);
        newToggle.GetComponent<CollectionToggle>().index = cardListToggles.IndexOf(newToggle);

        if (!isStart) playerDecks.Add(new List<Card>());

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
        if(list.currentPage == 1 && i == -1) return;
        if(list.currentPage == list.totalPages && i == 1) return;
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
    private void CreateButtonCallback()
    {
        CreateNewDeck();
    }

    // Bound to an onClick event from the "Set Active Button'. Calls SetActiveDeck function
    private void SetActiveButtonCallback()
    {
        SetActiveDeckToDB();
    }
}
