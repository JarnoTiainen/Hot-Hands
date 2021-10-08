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
    [SerializeField] private int playerStartHealth = 100;

    [SerializeField] private CardList cardList;
    private GameObject sfxLibrary;

    private void Awake()
    {
        Instance = gameObject.GetComponent<GameManager>();
        sfxLibrary = GameObject.Find("SFXLibrary");
        playerStats = new PlayerStats(playerStartHealth);
        enemyPlayerStats = new PlayerStats(playerStartHealth);
    }

    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

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
            Vector3 cardPos = References.i.opponentBonfire.transform.position;
            cardPos.z = Hand.Instance.gameObject.transform.position.z;
            newCard = Instantiate(References.i.handCard, cardPos, Quaternion.Euler(0, 180, 0));
            EnemyHand.Instance.RemoveCard(burnCardMessage.handIndex);
            CardData cardData = References.i.cardList.GetCardData(cardMessage);
            newCard.GetComponent<InGameCard>().cardData = cardData;
            PlayerBurnCard(newCard, cardMessage.player);
        }
        
    }

    public void PlayerBurnCard(GameObject card, int player = -1)
    {
        if (player == -1) player = playerNumber;
        sfxLibrary.GetComponent<BurnSFX>().Play();
        
        int value = card.GetComponent<InGameCard>().cardData.value;
        if (player == playerNumber)
        {
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            card.transform.SetParent(References.i.opponentBonfire.transform);
            UpdatePlayerBurnValue(player, enemyPlayerStats.playerBurnValue + value);
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        card.GetComponent<InGameCard>().Burn();
    }
    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player == playerNumber)
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
            PlayerPlayCard(cardList.GetCardData(playCardMessage), playCardMessage.handIndex, playCardMessage.boardIndex, playCardMessage.player);
        }
    }
    public void PlayerPlayCard(CardData data, int handIndex = -1, int boardIndex = 0, int player = -1)
    {
        if (player == -1) player = playerNumber;
        if(player == playerNumber)
        {
            playerStats.playerBurnValue -= data.cost;
            References.i.yourMonsterZone.AddNewMonsterCard(true, boardIndex, data);
            Hand.Instance.RemoveCard(handIndex);
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue -= data.cost;
            References.i.opponentMonsterZone.AddNewMonsterCard(false, boardIndex, data);
            //for now removes card with index 0
            Debug.Log("Removing card from hand");
            EnemyHand.Instance.RemoveCard(0);
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
            if (wasYourAttack)
            {
                References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
                References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, target);
                if (attacker.lp <= 0 || attacker.rp <= 0) References.i.yourMonsterZone.RemoveMonsterCard(attacker.index);
                if (target.lp <= 0 || target.rp <= 0) References.i.opponentMonsterZone.RemoveEnemyMonsterCard(target.index);
            }
            else
            {
                References.i.yourMonsterZone.UpdateCardData(!wasYourAttack, target);
                References.i.opponentMonsterZone.UpdateCardData(wasYourAttack, attacker);
                if (attacker.lp <= 0 || attacker.rp <= 0) References.i.yourMonsterZone.RemoveMonsterCard(target.index);
                if (target.lp <= 0 || target.rp <= 0) References.i.opponentMonsterZone.RemoveEnemyMonsterCard(attacker.index);
            }
            
        }
    }
}
