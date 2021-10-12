using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Deck : MonoBehaviour, IOnClickDownUIElement
{
    public List<Card> playerDeck = new List<Card>();
    [SerializeField] private float cardCooldown;
    [SerializeField] private bool OnPreDrawCD = false;
    [SerializeField] private bool cardDrawReady = true;
    [SerializeField] private bool debuggerModeOn = false;

    private GameObject gameManager;

    [Button] public void SendDeckData()
    {
        WebSocketService.SetDeck(GetDeckJSON());
    }

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    public void OnClickElement()
    {
        //let the player pick a card if the deck is set
        if(gameManager.GetComponent<GameManager>().deckSet) {
            if(GameManager.Instance.playerStats.playerHandCards < GameManager.Instance.maxHandSize)
            {
                if(cardDrawReady && !OnPreDrawCD)
                {
                    OnPreDrawCD = true;
                    WebSocketService.DrawCard();
                    GameManager.Instance.playerStats.playerHandCards++;
                    GameManager.Instance.PlayerDrawCard();
                }
                else
                {
                    if(debuggerModeOn) Debug.Log("Draw on cd");
                }
            }
            else
            {
                //Debug.LogWarning("Hand full display error effect here");
            }
        }
        
    }

    public string GetDeckJSON()
    {
        DeckObject deck = new DeckObject(playerDeck);
        return JsonUtility.ToJson(deck);
    }

    public void StartDrawCooldown(float duration)
    {
        OnPreDrawCD = false;
        cardDrawReady = false;
        if (debuggerModeOn) Debug.Log("Draw cooldown started: " + duration);
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
        else if(cardCooldown <= 0 && !OnPreDrawCD)
        {
            FinisheDrawCooldown();
        }
    }
}
