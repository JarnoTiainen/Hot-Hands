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

    [SerializeField] private float cardCooldown;
    [SerializeField] private bool OnPreDrawCD = false;
    [SerializeField] private bool cardDrawReady = true;

    private void Awake()
    {
        cardPrefab = References.i.fieldCard;
    }
    private void Start()
    {
        deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
        CreateDeck();
    }
    public void Update()
    {
        if (cardCooldown > 0) cardCooldown -= Time.deltaTime;
        else if (cardCooldown <= 0 && !OnPreDrawCD)
        {
            FinisheDrawCooldown();
        }
    }

    [Button] public void CreateDeck()
    {
        deckCardsCount = GameManager.Instance.playerStats.deckCardCount;
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

    public void OnClickElement()
    {
        if(GameManager.Instance.IsYou(owner))
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
        OnPreDrawCD = false;
        cardDrawReady = false;
        cardCooldown = duration;
    }

    public void FinisheDrawCooldown()
    {
        cardDrawReady = true;
        cardCooldown = 0;
    }
}
