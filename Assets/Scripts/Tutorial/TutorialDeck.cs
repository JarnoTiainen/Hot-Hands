using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDeck : MonoBehaviour, IOnClickDownUIElement
{
    private TutorialManager tutorialManager;
    [SerializeField] private List<Card> cards;
    
    [SerializeField] private float drawCooldown;
    private Queue<Card> cardsQueue;

    [SerializeField] private float cardCooldown;
    [SerializeField] private bool cardDrawReady = true;
    private int drawnCards;

    private void Awake()
    {
        cardsQueue = new Queue<Card>();

        tutorialManager = TutorialManager.tutorialManagerInstance;

        foreach (Card card in cards ) {
            cardsQueue.Enqueue(card);
        }
    }

    private void Start()
    {
        drawnCards = 0;
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

    public void OnClickElement()
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
}
