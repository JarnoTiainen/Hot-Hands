using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private bool debuggerModeOn;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerBurnCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerDrawCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerPlayCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerAttack;

    public int playerNumber;
    public PlayerStats playerStats;
    public PlayerStats enemyPlayerStats;
    [SerializeField] private int playerStartHealth = 100;
    public int maxHandSize = 5;
    public int maxFieldCardCount = 5;
    public int maxDeckSize = 20;
    public bool deckSet = false;

    private GameObject sfxLibrary;

    [SerializeField] private List<GameObject> unHandledBurnedCards = new List<GameObject>();

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
        if(burnCardMessage.denied)
        {
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue - unHandledBurnedCards[0].GetComponent<InGameCard>().cardData.value);
            ReturnBurnedCardToHand();
            return;
        }


        burnCardMessage.burnedCardDone = JsonUtility.FromJson<DrawCardMessage>(burnCardMessage.burnedCard);
        DrawCardMessage cardMessage = burnCardMessage.burnedCardDone;
        if (cardMessage.player == playerNumber)
        {
            Debug.Log("removing ");
            unHandledBurnedCards.Remove(unHandledBurnedCards[0]);
            playerStats.playerHandCards--;
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
            enemyPlayerStats.playerBurnValue = burnCardMessage.newBurnValue;
            PlayerBurnCard(newCard, cardMessage.player);
        }
        
    }
    public void ReturnBurnedCardToHand()
    {
        Debug.Log("Ret removing ");
        Hand.AddNewCardToHand(unHandledBurnedCards[0]);
        unHandledBurnedCards.Remove(unHandledBurnedCards[0]);
    }

    public void PlayerBurnCard(GameObject card, int player = -1)
    {
        if (player == -1) player = playerNumber;
        sfxLibrary.GetComponent<BurnSFX>().Play();
        
        int value = card.GetComponent<InGameCard>().cardData.value;
        if (player == playerNumber)
        {
            Debug.Log("adding ");
            unHandledBurnedCards.Add(card);
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            card.transform.SetParent(References.i.opponentBonfire.transform);
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        card.GetComponent<InGameCard>().Burn();
    }
    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player == playerNumber)
        {   
            playerStats.playerBurnValue = newValue;
            GameObject.Find("Bonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue = newValue;
            GameObject.Find("OpponentBonfire").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
    }


    public void PlayerDrawCard(DrawCardMessage drawCardMessage)
    {
        if (drawCardMessage.player == playerNumber)
        {
            Hand.RevealNewCard(drawCardMessage);
            References.i.yourDeckObj.GetComponent<Deck>().StartDrawCooldown(drawCardMessage.drawCooldown);
        }
        else
        {
            PlayerDrawCard(drawCardMessage.player);
        }
    }
    public void PlayerDrawCard(int player = -1)
    {
        if(player == -1)
        {
            player = playerNumber;
        }
        if(player == playerNumber)
        {
            playerStats.deckCardCount--;
            Hand.AddNewCard();
        }
        else
        {
            
            enemyPlayerStats.deckCardCount--;
            Debug.Log("Remaining cards " + enemyPlayerStats.deckCardCount);
            EnemyHand.AddNewCard();
        }
        sfxLibrary.GetComponent<DrawCardSFX>().Play();
        
    }

    [Button]public void PlayerReturnDrawCard()
    {
        Hand.RemoveHiddenCard();
        References.i.yourDeckObj.GetComponent<Deck>().StartDrawCooldown(0);
        playerStats.deckCardCount++;
        playerStats.playerHandCards--;
    }

    public void SetDeck(SetDeckMessage setDeckMessage)
    {
        if(setDeckMessage.player == playerNumber)
        {
            Debug.Log("Setting your deck");
            playerStats.deckCardCount = setDeckMessage.deckCards;
            deckSet = true;
        }
        else
        {
            Debug.Log("Setting opponent's deck");
            enemyPlayerStats.deckCardCount = setDeckMessage.deckCards;
        }
    }

    public void PlayerPlayCard(PlayCardMessage playCardMessage)
    {
        if(playCardMessage.denied)
        {
            playerStats.playerFieldCards--;
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue + References.i.yourMonsterZone.unhandledCards[0].GetComponent<InGameCard>().cardData.cost);
            References.i.yourMonsterZone.RecallCard(playerNumber, References.i.yourMonsterZone.unhandledCards[0]);
            return;
        }

        if (playCardMessage.player == playerNumber)
        {
            
            References.i.yourMonsterZone.UpdateCardData(true, playCardMessage);
            References.i.yourMonsterZone.GetCardWithServerIndex(playCardMessage.boardIndex).GetComponent<InGameCard>().StartAttackCooldown(playCardMessage.attackCooldown, true);
            if ((PlayCardMessage.CardSource)playCardMessage.cardSource == PlayCardMessage.CardSource.Hand)
            {
                //display summon animation here
                playerStats.playerHandCards--;
            }
            else if((PlayCardMessage.CardSource)playCardMessage.cardSource == PlayCardMessage.CardSource.Deck)
            {
                //display summon animation here
                playerStats.deckCardCount--;
            }
            else if ((PlayCardMessage.CardSource)playCardMessage.cardSource == PlayCardMessage.CardSource.DiscardPile)
            {
                //display summon animation here
                playerStats.discardpileCardCount--;
            }
        }
        else
        {
            PlayerPlayCard(References.i.cardList.GetCardData(playCardMessage), playCardMessage.handIndex, playCardMessage.boardIndex, playCardMessage.player);
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

    public void PlayerRecallCard() { 
        
    }

    public void PlayerAttack(AttackEventMessage attackEventMessage)
    {
        attackEventMessage.attackerValues = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.attacker);
        CardPowersMessage attacker = attackEventMessage.attackerValues;
        if (attackEventMessage.attackDenied)
        {
            AttackDenied(attacker);
            return;
        }

        
        CardPowersMessage target = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.target);
        if (attackEventMessage.directHit)
        {
            if (attackEventMessage.player == playerNumber)
            {
                References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index).GetComponent<InGameCard>().StartAttackCooldown(attackEventMessage.attackCooldown);
                enemyPlayerStats.playerHealth -= attackEventMessage.playerTakenDamage;
                if(debugPlayerAttack) Debug.Log("Enemy lost " + attackEventMessage.playerTakenDamage + " health. New health is: " + enemyPlayerStats.playerHealth);
            }
            else
            {
                References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index)).GetComponent<InGameCard>().StartAttackCooldown(attackEventMessage.attackCooldown);
                playerStats.playerHealth -= attackEventMessage.playerTakenDamage;
                if (debugPlayerAttack) Debug.Log("You lost " + attackEventMessage.playerTakenDamage + " health. New health is: " + playerStats.playerHealth);
            }
        }
        else
        {
            
            attackEventMessage.targetValues = target;

            if (debugPlayerAttack) Debug.Log("attacked index: " + attackEventMessage.attackerValues.index + " target index: " + attackEventMessage.targetValues.index + "attacker lp: " + attackEventMessage.attackerValues.lp + " rp: " + attackEventMessage.attackerValues.rp + " target lp: " + attackEventMessage.targetValues.lp + " rp: " + attackEventMessage.targetValues.rp);
            bool wasYourAttack = false;
            if (attackEventMessage.player == playerNumber) wasYourAttack = true;

            Debug.LogWarning("was your attack " + wasYourAttack);
            if (wasYourAttack)
            {
                References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index).GetComponent<InGameCard>().StartAttackCooldown(attackEventMessage.attackCooldown);
                References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
                References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, target);
                if (attacker.lp <= 0 || attacker.rp <= 0)
                {
                    playerStats.playerFieldCards--;
                    References.i.yourMonsterZone.RemoveMonsterCard(attacker.index);
                }
                if (target.lp <= 0 || target.rp <= 0) References.i.opponentMonsterZone.RemoveEnemyMonsterCard(target.index);
            }
            else
            {
                Debug.Log("index: " + References.i.opponentMonsterZone.RevertIndex(attacker.index));
                References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index)).GetComponent<InGameCard>().StartAttackCooldown(attackEventMessage.attackCooldown);
                References.i.yourMonsterZone.UpdateCardData(!wasYourAttack, target);
                References.i.opponentMonsterZone.UpdateCardData(wasYourAttack, attacker);
                if (attacker.lp <= 0 || attacker.rp <= 0)
                {
                    References.i.opponentMonsterZone.RemoveEnemyMonsterCard(attacker.index);
                }
                if (target.lp <= 0 || target.rp <= 0)
                {
                    playerStats.playerFieldCards--;
                    References.i.yourMonsterZone.RemoveMonsterCard(target.index);
                }
            }
            
        }
    }
    public void AttackDenied(CardPowersMessage attacker)
    {
        Debug.LogError("ATTACK DENIED");
        References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index).GetComponent<InGameCard>().StartAttackCooldown(0);
    }
}
