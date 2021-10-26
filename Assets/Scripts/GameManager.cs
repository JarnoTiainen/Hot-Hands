using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerStats playerStats;
    public PlayerStats enemyPlayerStats;
    public float attackDuration = 0.3f;
    public float moveDuration = 0.4f;
    public float rearrangeDuration = 0.4f;
    public int playerNumber;
    public int maxHandSize = 5;
    public int maxFieldCardCount = 5;
    public int maxDeckSize = 20;
    public bool deckSet = true;
    
    [SerializeField] private List<GameObject> unHandledBurnedCards = new List<GameObject>();
    [SerializeField] private int playerStartHealth = 100;
    [SerializeField] private bool debuggerModeOn;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerBurnCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerDrawCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerPlayCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerAttack;

    private GameObject sfxLibrary;

    

    private void Awake()
    {
        Instance = gameObject.GetComponent<GameManager>();
        playerStats = new PlayerStats(playerStartHealth);
        enemyPlayerStats = new PlayerStats(playerStartHealth);
    }

    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    public void SetNewSFXLibrary(GameObject SFXLibrary)
    {
        sfxLibrary = SFXLibrary;
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
        if (debugPlayerBurnCard) Debug.Log("Ret removing ");
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
            if(debugPlayerBurnCard) Debug.Log("adding ");
            unHandledBurnedCards.Add(card);
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            References.i.yourBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            card.transform.SetParent(References.i.opponentBonfire.transform);
            References.i.opponentBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        card.GetComponent<InGameCard>().Burn();
    }
    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player == playerNumber)
        {   
            playerStats.playerBurnValue = newValue;
            References.i.yourBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue = newValue;
            References.i.opponentBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = enemyPlayerStats.playerBurnValue.ToString();
        }
    }

    public void PlayerGainBurnValue(GainBurnValueMessage gainBurnValueMessage)
    {
        if(IsYou(gainBurnValueMessage.player))
        {
            UpdatePlayerBurnValue(gainBurnValueMessage.player, playerStats.playerBurnValue + gainBurnValueMessage.gainedBurnValue);
        }
        else UpdatePlayerBurnValue(gainBurnValueMessage.player, enemyPlayerStats.playerBurnValue + gainBurnValueMessage.gainedBurnValue);
    }


    public void PlayerDrawCard(DrawCardMessage drawCardMessage)
    {
        if (drawCardMessage.player == playerNumber)
        {
            if(drawCardMessage.drawCooldown == -1)
            {
                PlayerDrawCard();
                GameManager.Instance.playerStats.playerHandCards++;
            }
            else
            {
                References.i.yourDeckObj.GetComponent<Deck>().StartDrawCooldown(drawCardMessage.drawCooldown);
            }
            Hand.RevealNewCard(drawCardMessage);

        }
        else
        {
            PlayerDrawCard(drawCardMessage.player);
        }
    }

    public void EnchantmentEffectActive(EnchantmentEffectMesage enchantmentEffect)
    {
        Debug.Log("Display effect here");
        enchantmentEffect.cardInfo = JsonUtility.FromJson<DrawCardMessage>(enchantmentEffect.card);
        Debug.Log(enchantmentEffect.cardInfo.cardName + " " + enchantmentEffect.trigger);


         References.i.cardEnchantmentEffectManager.DisplayCardEffectSource(References.i.cardList.GetCardData(enchantmentEffect.cardInfo));
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
            if(debugPlayerDrawCard) Debug.Log("Remaining cards " + enemyPlayerStats.deckCardCount);
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
            playerStats.deckCardCount = setDeckMessage.deckCards;
            deckSet = true;
        }
        else
        {
            enemyPlayerStats.deckCardCount = setDeckMessage.deckCards;
        }
    }

    public void PlayerPlayCard(PlayCardMessage playCardMessage)
    {
        if(playCardMessage.denied)
        {
            playerStats.playerFieldCards--;
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue + References.i.yourMonsterZone.unhandledCards[0].GetComponent<InGameCard>().cardData.cost);
            if (playCardMessage.serverBurnValue != playerStats.playerBurnValue) UpdatePlayerBurnValue(playerNumber, playCardMessage.serverBurnValue);
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
            References.i.yourMonsterZone.GetCardWithServerIndex(playCardMessage.boardIndex).GetComponent<InGameCard>().owner = playCardMessage.player;
        }
        else
        {
            PlayerPlayCard(References.i.cardList.GetCardData(playCardMessage), playCardMessage.handIndex, playCardMessage.boardIndex, playCardMessage.player, playCardMessage.attackCooldown);
        }
    }
    public void PlayerPlayCard(CardData data, int handIndex = -1, int boardIndex = 0, int player = -1, float attackCD = 0)
    {
        if (player == -1) player = playerNumber;
        if(player == playerNumber)
        {
            playerStats.playerBurnValue -= data.cost;
            References.i.yourMonsterZone.AddNewMonsterCard(true, boardIndex, data);
            Hand.Instance.RemoveCard(handIndex);
            References.i.yourBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI > ().text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue -= data.cost;
            References.i.opponentMonsterZone.AddNewMonsterCard(false, boardIndex, data);
            References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(boardIndex)).GetComponent<InGameCard>().StartAttackCooldown(attackCD, true);
            References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(boardIndex)).GetComponent<InGameCard>().owner = player;
            //for now removes card with index 0
            if(debugPlayerPlayCard) Debug.Log("Removing card from hand");
            EnemyHand.Instance.RemoveCard(0);
            References.i.opponentBonfire.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI > ().text = enemyPlayerStats.playerBurnValue.ToString();
        }
        sfxLibrary.GetComponent<PlayCardSFX>().Play();
    }

    public void PlayerAttack(AttackEventMessage attackEventMessage)
    {
        

        attackEventMessage.attackerValues = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.attacker);
        CardPowersMessage attacker = attackEventMessage.attackerValues;
        if (attackEventMessage.denied)
        {
            if(debugPlayerAttack) Debug.LogError("ATTACK DENIED");
            AttackDenied(attacker);
            return;
        }

        
        CardPowersMessage target = JsonUtility.FromJson<CardPowersMessage>(attackEventMessage.target);
        if (attackEventMessage.directHit)
        {
            if (attackEventMessage.player == playerNumber)
            {
                References.i.attackEventHandler.StartAttackEvent(true, attacker, attackEventMessage.playerTakenDamage, attackEventMessage.attackCooldown);
            }
            else
            {    
                References.i.attackEventHandler.StartAttackEvent(false, attacker, attackEventMessage.playerTakenDamage, attackEventMessage.attackCooldown);
            }
        }
        else
        {
            attackEventMessage.targetValues = target;
            if (debugPlayerAttack) Debug.Log("attacked index: " + attackEventMessage.attackerValues.index + " target index: " + attackEventMessage.targetValues.index + "attacker lp: " + attackEventMessage.attackerValues.lp + " rp: " + attackEventMessage.attackerValues.rp + " target lp: " + attackEventMessage.targetValues.lp + " rp: " + attackEventMessage.targetValues.rp);
            bool wasYourAttack = false;
            if (attackEventMessage.player == playerNumber) wasYourAttack = true;
            if (debugPlayerAttack) Debug.LogWarning("was your attack " + wasYourAttack);
            References.i.attackEventHandler.StartAttackEvent(wasYourAttack, attacker, target, attackEventMessage.attackCooldown);
        }
    }

    public bool IsYou(int player)
    {
        if (player == playerNumber) return true;
        else return false;
    }
    public void AttackDenied(CardPowersMessage attacker)
    {
        References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index).GetComponent<InGameCard>().StartAttackCooldown(0);
    }
}
