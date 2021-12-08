using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }
    private CardList resourcesCardList;
    public List<CollectionMenuButtonManager> deckToggles = new List<CollectionMenuButtonManager>();
    public List<List<Card>> playerDecks = new List<List<Card>>();
    public List<string> deckNames = new List<string>();
    public GameObject collectionCardList;
    [SerializeField] private GameObject pageText;
    [SerializeField] private GameObject collectionMenu;
    [SerializeField] private RenameDeckPopup renameDeckPopup;
    public int playerDeckLimit = 5;
    private float collectionX;
    private float collectionY;
    private float collectionZ;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        collectionMenu.SetActive(true);
        collectionX = collectionMenu.transform.position.x;
        collectionY = collectionMenu.transform.position.y;
        collectionZ = collectionMenu.transform.position.z;
        MoveCollectionMenu(false);
        resourcesCardList = Resources.Load("Card List") as CardList;
        for(int i = 0; playerDeckLimit > i; i++)
        {
            deckNames.Add("");
        }
        SetAllCardsList();
        UpdatePageText();
    }

    public void MoveCollectionMenu(bool visible)
    {
        if(visible) collectionMenu.transform.position = new Vector3(collectionX, collectionY, collectionZ);
        else collectionMenu.transform.position = new Vector3(1000, collectionY, -1000);
    }

    public void RenamePopupSetActive(bool value)
    {
        int deckIndex = -1;
        for (int i = 0; deckToggles.Count > i; i++)
        {
            if (deckToggles[i].deckSelected)
            {
                deckIndex = i;
                break;
            }
        }
        if (deckIndex == -1) return;
        renameDeckPopup.gameObject.SetActive(value);
    }

    public void SetPlayerDecks(GetDecksMessage getDecksMessage)
    {
        // Monke
        List<List<string>> playerDecksTemp = new List<List<string>>();
        playerDecksTemp.Add(getDecksMessage.deck0);
        playerDecksTemp.Add(getDecksMessage.deck1);
        playerDecksTemp.Add(getDecksMessage.deck2);
        playerDecksTemp.Add(getDecksMessage.deck3);
        playerDecksTemp.Add(getDecksMessage.deck4);
        deckNames[0] = getDecksMessage.deck0Name;
        deckNames[1] = getDecksMessage.deck1Name;
        deckNames[2] = getDecksMessage.deck2Name;
        deckNames[3] = getDecksMessage.deck3Name;
        deckNames[4] = getDecksMessage.deck4Name;

        foreach (List<string> deck in playerDecksTemp)
        {
            List<Card> tempDeck = new List<Card>();
            foreach(string cardName in deck)
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
            if (deckNames[i] == null || deckNames[i] == "") deckToggles[i].ChangeDeckName("Deck " + (i + 1));
            else deckToggles[i].ChangeDeckName(deckNames[i]);
        }

        int activeDeck = getDecksMessage.activeDeckIndex;
        deckToggles[activeDeck].ToggleButton();
        deckToggles[activeDeck].ToggleDeckSelected();
        DeckBuilder.Instance.OpenDeck(activeDeck);
        collectionMenu.SetActive(false);
    }

    // Sends the players active deck index to the db and visually shows which deck is active
    public void SetActiveDeckToDB()
    {
        int deckIndex = -1;
        for (int i = 0; deckToggles.Count > i; i++)
        {
            if (deckToggles[i].deckSelected)
            {
                deckIndex = i;
                break;
            }
        }
        if (deckIndex == -1) return;
        if(playerDecks[deckIndex].Count < DeckBuilder.Instance.deckSizeLimit)
        {
            MainMenu.Instance.CreatePopupNotification("Make sure the deck is full before trying to set it active!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
            return;
        }
        WebSocketService.SetActiveDeck(deckIndex);
        deckToggles[deckIndex].ToggleDeckSelected();
        MainMenu.Instance.CreatePopupNotification("Active deck changed.", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Positive);
    }

    // Sends the players deck data to the db
    public void SaveDeckToDB(int index, bool rename = false)
    {
        DeckObject deckString;
        if (!rename)
        {
            deckString = new DeckObject(playerDecks[index], index, deckNames[index]);
            MainMenu.Instance.CreatePopupNotification("Deck saved.", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Positive);
        }
        else
        {
            int deckIndex = -1;
            for (int i = 0; deckToggles.Count > i; i++)
            {
                if (deckToggles[i].deckSelected)
                {
                    deckIndex = i;
                    break;
                }
            }
            if (deckIndex == -1) return;
            if(renameDeckPopup.deckNameInput.text.Length == 0)
            {
                MainMenu.Instance.CreatePopupNotification("Deck name can't be empty!", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Negative);
                return;
            }
            string newName = renameDeckPopup.deckNameInput.text;
            renameDeckPopup.deckNameInput.text = "";
            deckNames[deckIndex] = newName;
            deckToggles[deckIndex].ChangeDeckName(newName);
            deckString = new DeckObject(playerDecks[deckIndex], deckIndex, newName);
            renameDeckPopup.gameObject.SetActive(false);
            MainMenu.Instance.CreatePopupNotification("Deck renamed.", MainMenu.PopupCorner.TopRight, MainMenu.PopupTone.Positive);
        }
        WebSocketService.SaveDeck(JsonUtility.ToJson(deckString));
    }

    // Gets all game cards and passes them to the 'All Cards List' and calls it's populate function
    private void SetAllCardsList()
    {
        // Add all cards in the resources allCards-list to Collection's "All Cards"
        CollectionCardList cardListScript = collectionCardList.GetComponent<CollectionCardList>();
        foreach (CardList.ListCard listCard in resourcesCardList.allCards)
        {
            cardListScript.cards.Add(listCard.card);
        }
        // Sort alphabetically
        cardListScript.SortList(CollectionCardList.SortMethod.Name);
        cardListScript.PopulatePage(1);
    }

    // Changes the page on the active ingame list
    public void ChangePage(int i)
    {
        CollectionCardList list = collectionCardList.GetComponent<CollectionCardList>();
        if(list.currentPage == 1 && i == -1) return;
        if(list.currentPage == list.totalPages && i == 1) return;
        list.PopulatePage(list.currentPage + i);
        UpdatePageText();
    }

    // Updates the page counter
    public void UpdatePageText()
    {
        CollectionCardList list = collectionCardList.GetComponent<CollectionCardList>();
        pageText.GetComponent<TextMeshProUGUI>().text = (list.currentPage) + "/" + list.totalPages;
    }
}
