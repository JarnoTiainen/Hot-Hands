using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    [SerializeField] private bool debuggerModeOn;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerBurnCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerDrawCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerPlayCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerAttack;

    private int playerNumber;
    public PlayerStats playerStats;
    public PlayerStats enemyPlayerStats;
    [SerializeField] private CardList cardList;


    [SerializeField] private int playerStartHealth = 100;

    private GameObject sfxLibrary;

    private void Awake()
    {
        Instance = gameObject.GetComponent<GameManager>();
        sfxLibrary = GameObject.Find("SFXLibrary");
    }

    private void Start()
    {
        playerStats = new PlayerStats(playerStartHealth);
        enemyPlayerStats = new PlayerStats(playerStartHealth);
    }


    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    //Message from server is directed trough this code before  the actual function
    public void PlayerBurnCard(BurnCardMessage burnCardMessage)
    {

        burnCardMessage.burnedCardDone = JsonUtility.FromJson<DrawCardMessage>(burnCardMessage.burnedCard);
        DrawCardMessage cardMessage = burnCardMessage.burnedCardDone;
        if (cardMessage.player == playerNumber)
        {
            
        }
        else
        {
            GameObject newCard;
            newCard = Instantiate(References.i.handCard, References.i.opponentBonfire.transform.position, Quaternion.Euler(0, 180, 0));
            Hand.Instance.RemoveCard(burnCardMessage.handIndex);
            CardData cardData = References.i.cardList.GetCardData(cardMessage);
            newCard.GetComponent<InGameCard>().cardData = cardData;
            PlayerBurnCard(newCard, cardMessage.player);
        }
        
    }
    //Trigger sound effect and all that stuff
    //Called from server for enemy player and from Mouse script for client owener
    public void PlayerBurnCard(GameObject card, int player = -1)
    {
        if (player == -1) player = playerNumber;
        sfxLibrary.GetComponent<BurnSFX>().Play();
        
        int value = card.GetComponent<InGameCard>().cardData.value;
        if (player == playerNumber)
        {
            Debug.LogWarning("START YOUR BURN EFFECT HERE FOR THE CARD");
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            Debug.LogWarning("START YOUR BURN EFFECT HERE FOR THE CARD");
            card.transform.SetParent(References.i.opponentBonfire.transform);
            UpdatePlayerBurnValue(player, enemyPlayerStats.playerBurnValue + value);
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        card.GetComponent<InGameCard>().Burn();
    }
    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player ==playerNumber)
        {
            playerStats.playerBurnValue = newValue;
        }
        else
        {
            enemyPlayerStats.playerBurnValue = newValue;
        }
    }


    public void PlayerDrawCard(DrawCardMessage drawCardMessage)
    {
        if (drawCardMessage.player == playerNumber)
        {
            Hand.RevealNewCard(drawCardMessage);
        }
        else
        {
            EnemyHand.AddNewCard();
            PlayerDrawCard();
        }
    }
    public void PlayerDrawCard()
    {
        sfxLibrary.GetComponent<DrawCardSFX>().Play();
    }


    public void PlayerPlayCard(PlayCardMessage playCardMessage)
    {
        if (playCardMessage.player == playerNumber)
        {
            References.i.yourMonsterZone.UpdateCardData(true, playCardMessage);
        }
        else
        {
            References.i.opponentMonsterZone.AddNewMonsterCard(false, playCardMessage.boardIndex, cardList.GetCardData(playCardMessage));
            enemyPlayerStats.playerBurnValue -= playCardMessage.cardCost;
            PlayerPlayCard(playCardMessage.player);
        }
    }
    public void PlayerPlayCard(int player = -1)
    {
        if (player == -1) player = playerNumber;
        if(player == playerNumber)
        {
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        sfxLibrary.GetComponent<PlayCardSFX>().Play();
    }

    public void PlayerAttack(AttackEventMessage attackEventMessage)
    {
        attackEventMessage.attackerValues = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.attacker);
        if (attackEventMessage.directHit)
        {
            if (attackEventMessage.player == playerNumber)
            {
                enemyPlayerStats.playerHealth -= attackEventMessage.playerTakenDamage;
                if(debugPlayerAttack) Debug.Log("Enemy lost " + attackEventMessage.playerTakenDamage + " health. New health is: " + enemyPlayerStats.playerHealth);
            }
            else
            {
                playerStats.playerHealth -= attackEventMessage.playerTakenDamage;
                if (debugPlayerAttack) Debug.Log("You lost " + attackEventMessage.playerTakenDamage + " health. New health is: " + playerStats.playerHealth);
            }
        }
        else
        {
            CardPowersMessage attacker = attackEventMessage.attackerValues;
            CardPowersMessage target = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.target);
            attackEventMessage.targetValues = target;

            if (debugPlayerAttack) Debug.Log("attacked index: " + attackEventMessage.attackerValues.index + " target index: " + attackEventMessage.targetValues.index + "attacker lp: " + attackEventMessage.attackerValues.lp + " rp: " + attackEventMessage.attackerValues.rp + " target lp: " + attackEventMessage.targetValues.lp + " rp: " + attackEventMessage.targetValues.rp);
            bool wasYourAttack = false;
            if (attackEventMessage.player == playerNumber) wasYourAttack = true;
            References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
            References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, attacker);
            if (attacker.lp <= 0 || attacker.rp <= 0) References.i.yourMonsterZone.RemoveMonsterCard(attacker.index);
            if (target.lp <= 0 || target.rp <= 0) References.i.opponentMonsterZone.RemoveEnemyMonsterCard(target.index);
        }
    }
}
