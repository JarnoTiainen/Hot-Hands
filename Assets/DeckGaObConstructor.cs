using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeckGaObConstructor : MonoBehaviour, IOnClickDownUIElement
{
    GameObject cardPrefab;
    [SerializeField] private float cardOverlapAmount;
    [SerializeField] private int deckCardsCount;
    [SerializeField] List<GameObject> deckCards = new List<GameObject>();
    [SerializeField] private float cardZrotationOffset = 5;
    [SerializeField] private float cardPosOffset = 0.01f;
    [SerializeField] private int owner;
    [SerializeField] private bool yourDeck;

    [SerializeField] private float cardCooldown;
    [SerializeField] private bool OnPreDrawCD = false;
    [SerializeField] private bool cardDrawReady = true;
    [SerializeField] private bool interactable = false;
    [SerializeField] private GameObject discardPile;

    private int i;

    private void Start()
    {
        cardPrefab = References.i.fieldCard;
        if (owner == 0) deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
        else if(owner == 1) deckCardsCount = GameManager.Instance.enemyPlayerStats.deckCardCount;
        CreateDeck();

        if (yourDeck) owner = GameManager.Instance.playerNumber;
        else owner = 1 - GameManager.Instance.playerNumber;
    }
    public void Update()
    {
        if (cardCooldown > 0) cardCooldown -= Time.deltaTime;
        else if (cardCooldown <= 0 && !OnPreDrawCD && !cardDrawReady)
        {
            FinisheDrawCooldown();
        }
    }

    public void SetOwner(int i)
    {
        owner = i;
    }

    [Button] public void CreateDeck()
    {
        foreach(GameObject deckCard in deckCards)
        {
            Destroy(deckCard);
        }
        deckCards = new List<GameObject>();
        if (owner == 0) deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
        else if (owner == 1) deckCardsCount = GameManager.Instance.enemyPlayerStats.deckCardCount;
        i = 0;
        InvokeRepeating("AddCardToDeck", 0, 0.1f);
    }



    public void AddCardToDeck()
    {
        if (deckCards.Count == deckCardsCount)
        {
            if(cardDrawReady)
            {
                foreach (GameObject card in deckCards)
                {
                    card.GetComponent<InGameCard>().StartDrawCardReadyEffect();
                }
            }
            interactable = true;
            CancelInvoke();
            return;
        }
        
        GameObject newDeckCard = Instantiate(cardPrefab);
        //Set position to burnpile
        newDeckCard.transform.SetParent(transform);
        newDeckCard.transform.rotation = Quaternion.Euler(0, 180, Random.Range(-cardZrotationOffset, cardZrotationOffset));
        newDeckCard.transform.position = discardPile.transform.position;

        Vector3 finalCardPosition = new Vector3(Random.Range(-cardPosOffset, cardPosOffset), Random.Range(-cardPosOffset, cardPosOffset), -i * cardOverlapAmount);
        newDeckCard.GetComponent<CardMovement>().OnCardMove(newDeckCard.transform.localPosition, finalCardPosition, 0.3f);
        newDeckCard.GetComponent<InGameCard>().interActable = false;
        newDeckCard.GetComponent<InGameCard>().ReverseBurn();
        deckCards.Add(newDeckCard);
        i++;
    }

    public void AddSingleCardToDeck()
    {
        GameObject newDeckCard = Instantiate(cardPrefab);
        //Set position to burnpile
        newDeckCard.transform.SetParent(transform);
        newDeckCard.transform.rotation = Quaternion.Euler(0, 180, Random.Range(-cardZrotationOffset, cardZrotationOffset));
        if (owner == 0)
        {
            newDeckCard.transform.position = References.i.yourBurnPile.transform.position;
        }
        if (owner == 1)
        {
            newDeckCard.transform.position = References.i.opponentBurnPile.transform.position;
        }
        Vector3 finalCardPosition = new Vector3(Random.Range(-cardPosOffset, cardPosOffset), Random.Range(-cardPosOffset, cardPosOffset), -i * cardOverlapAmount);
        newDeckCard.GetComponent<CardMovement>().OnCardMove(newDeckCard.transform.localPosition, finalCardPosition, 0.3f);
        newDeckCard.GetComponent<InGameCard>().interActable = false;
        newDeckCard.GetComponent<InGameCard>().ReverseBurn();
        deckCards.Add(newDeckCard);
        i++;
    }



    [Button] public GameObject TakeTopCard()
    {
        GameObject topCard;
        if (deckCards.Count < 1)
        {
            topCard = Instantiate(cardPrefab);
            //Set position to burnpile
            topCard.transform.SetParent(transform);
            topCard.transform.rotation = Quaternion.Euler(0, 180, Random.Range(-cardZrotationOffset, cardZrotationOffset));
        }
        else
        {
            topCard = deckCards[deckCards.Count - 1];
        }
        deckCards.Remove(topCard);
        topCard.GetComponent<InGameCard>().StopDrawCardReadyEffect();
        return topCard;
    }

    public void OnClickElement()
    {
        if(yourDeck)
        {
            //let the player pick a card if the deck is set
            if (GameManager.Instance.deckSet)
            {
                if (GameManager.Instance.playerStats.playerHandCards < GameManager.Instance.maxHandSize)
                {
                    if (cardDrawReady && !OnPreDrawCD)
                    {
                        OnPreDrawCD = true;
                        WebSocketService.DrawCard();
                        GameManager.Instance.playerStats.playerHandCards++;
                        GameManager.Instance.PlayerDrawCard(GameManager.Instance.playerNumber);
                    }
                    else
                    {

                    }
                }
                else
                {
                    Debug.LogWarning("Hand full display error effect here");
                }
            }
        }
    }

    public void StartDrawCooldown(float duration)
    {
        Debug.Log("draw CD start");

        foreach (GameObject card in deckCards)
        {
            card.GetComponent<InGameCard>().StopDrawCardReadyEffect();
        }
        OnPreDrawCD = false;
        cardDrawReady = false;
        cardCooldown = duration;
    }

    public void FinisheDrawCooldown()
    {
        Debug.Log("draw CD end");

        foreach (GameObject card in deckCards)
        {
            card.GetComponent<InGameCard>().StartDrawCardReadyEffect();
        }
        cardDrawReady = true;
        cardCooldown = 0;
    }
}
