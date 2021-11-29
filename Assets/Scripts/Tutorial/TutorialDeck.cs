using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TutorialDeck : MonoBehaviour, IOnClickDownUIElement
{
    private TutorialManager tutorialManager;
    [SerializeField] private List<Card> cards;
    
    [SerializeField] private float drawCooldown;
    private Queue<Card> cardsQueue;

    [SerializeField] private float cardCooldown;
    [SerializeField] private bool cardDrawReady = true;
    private int drawnCards;
    GameObject cardPrefab;
    [SerializeField] private float cardOverlapAmount;
    [SerializeField] private int deckCardsCount;
    [SerializeField] List<GameObject> deckCards = new List<GameObject>();
    [SerializeField] private float cardZrotationOffset = 5;
    [SerializeField] private float cardPosOffset = 0.01f;
    [SerializeField] private int owner;

    private void Awake()
    {
        cardPrefab = References.i.fieldCard;

        cardsQueue = new Queue<Card>();

        tutorialManager = TutorialManager.tutorialManagerInstance;

        foreach (Card card in cards) {
            cardsQueue.Enqueue(card);
        }
        Debug.Log("owner " + owner + "deck count " + cardsQueue.Count);
    }

    private void Start()
    {
        if(owner == 0) deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
        else if(owner == 1) deckCardsCount = GameManager.Instance.enemyPlayerStats.deckCardCount;
        CreateDeck();

        drawnCards = 0;
    }

    [Button] public void CreateDeck()
    {
        if (owner == 0) deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
        else if (owner == 1) deckCardsCount = GameManager.Instance.enemyPlayerStats.deckCardCount;

        for (int i = 0; i < deckCardsCount; i++)
        {
            GameObject newDeckCard = Instantiate(cardPrefab);
            newDeckCard.transform.SetParent(transform);
            newDeckCard.transform.rotation = Quaternion.Euler(0, 180, Random.Range(-cardZrotationOffset, cardZrotationOffset));
            newDeckCard.transform.localPosition = new Vector3(Random.Range(-cardPosOffset, cardPosOffset), Random.Range(-cardPosOffset, cardPosOffset), -i * cardOverlapAmount);
            newDeckCard.GetComponent<InGameCard>().interActable = false;
            deckCards.Add(newDeckCard);
        }
    }

    [Button] public GameObject TakeTopCard()
    {
        GameObject topCard = deckCards[deckCards.Count-1];
        deckCards.Remove(topCard);
        return topCard;
    }



    public void StartDrawCooldown(float duration)
    {
        cardDrawReady = false;
        cardCooldown = duration;
        
    }

    public void FinisheDrawCooldown()
    {
        cardDrawReady = true;
        cardCooldown = 0;
    }

    public void Update()
    {
        if (cardCooldown > 0) cardCooldown -= Time.deltaTime;
        else if(cardCooldown <= 0)
        {
            FinisheDrawCooldown();
        }


    }

    public void OnClickElement2()
    { 
        if (tutorialManager.GetState() == TutorialManager.TutorialState.CardDraw && cardDrawReady) {
            if(cardsQueue.Count != 0) {
                Card drawnCard = cardsQueue.Dequeue();
                string seed = "0000000" + cardsQueue.Count.ToString();

                DrawCardMessage drawCardMessage = new DrawCardMessage(0, seed, drawCooldown, drawnCard);
                GameManager.Instance.PlayerDrawCard(0, seed);
                GameManager.Instance.PlayerDrawCard(drawCardMessage);
                drawnCards++;

                if(drawnCards == 2) {
                    tutorialManager.NextTutorialState();
                }
            } else {
                //reshuffle here?
                foreach (Card card in cards ) {
                    cardsQueue.Enqueue(card);
                }
            }
            
        }
    }

    public void OnClickElement()
    {
        if (tutorialManager.GetState() == TutorialManager.TutorialState.CardDraw) {
            if(GameManager.Instance.IsYou(owner))
            {
                //let the player pick a card if the deck is set
                if (GameManager.Instance.deckSet)
                {
                    if (GameManager.Instance.playerStats.playerHandCards < GameManager.Instance.maxHandSize)
                    {
                        if (cardDrawReady)
                        {
                            Card drawnCard = cardsQueue.Dequeue();
                            string seed = "0000000" + cardsQueue.Count.ToString();

                            DrawCardMessage drawCardMessage = new DrawCardMessage(0, seed, drawCooldown, drawnCard);
                            
                            GameManager.Instance.PlayerDrawCard(0, seed);
                            GameManager.Instance.PlayerDrawCard(drawCardMessage);
                            GameManager.Instance.playerStats.playerHandCards++;

                            if(GameManager.Instance.playerStats.playerHandCards == 2) {
                                tutorialManager.NextTutorialState();
                            }

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

    }

    public void OpponentDraw()
    { 
        if (GameManager.Instance.deckSet)
        {
            if (GameManager.Instance.playerStats.playerHandCards < GameManager.Instance.maxHandSize)
            {
                if (cardDrawReady)
                {
                    Card drawnCard = cardsQueue.Dequeue();
                    string seed = "1000000" + cardsQueue.Count.ToString();
                    Debug.Log("enemy seed " + seed);
                    Debug.Log("enemy card data " + drawnCard.cardName);
                    DrawCardMessage drawCardMessage = new DrawCardMessage(1, seed, drawCooldown, drawnCard);
                    //GameManager.Instance.PlayerDrawCard(1, seed);
                    
                    GameManager.Instance.PlayerDrawCard(drawCardMessage);
                    GameManager.Instance.enemyPlayerStats.playerHandCards++;
                    TutorialManager.tutorialManagerInstance.enemyCardSeeds.Add(seed);

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

    /*
     * if (tutorialManager.GetState() == TutorialManager.TutorialState.CardDraw && cardDrawReady) {
            if(cardsQueue.Count != 0) {
                Card drawnCard = cardsQueue.Dequeue();
                Debug.Log("name " + drawnCard.cardName);
                string seed = "0000001" + cardsQueue.Count.ToString();

                DrawCardMessage drawCardMessage = new DrawCardMessage(1, seed, drawCooldown, drawnCard);
                //GameManager.Instance.PlayerDrawCard(1, seed);
                GameManager.Instance.PlayerDrawCard(drawCardMessage);
            } else {
                //reshuffle here?
                foreach (Card card in cards ) {
                    cardsQueue.Enqueue(card);
                }
            }
            
        }
     */
}
