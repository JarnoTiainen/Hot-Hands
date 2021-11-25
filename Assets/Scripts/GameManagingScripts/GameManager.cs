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
    
    [SerializeField] private int playerStartHealth = 100;
    [SerializeField] private bool debuggerModeOn;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerBurnCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerDrawCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerPlayCard;
    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool debugPlayerAttack;

    private Line activeTargetLine;
    private CardData targettingCardData;

    private GameObject sfxLibrary;

    [SerializeField] private Dictionary<string, GameObject> inGameCards = new Dictionary<string, GameObject>();


    public void AddCardToInGameCards(GameObject newCard)
    {
        
        if(inGameCards.ContainsKey(newCard.GetComponent<InGameCard>().GetData().seed))
        {
            inGameCards.Remove(newCard.GetComponent<InGameCard>().GetData().seed);
            Debug.LogWarning("REadding " + newCard.GetComponent<InGameCard>().GetData().cardName + " to list");
        }
        else
        {
            Debug.LogWarning("adding " + newCard.GetComponent<InGameCard>().GetData().cardName + " to list");
        }
        inGameCards.Add(newCard.GetComponent<InGameCard>().GetData().seed, newCard);
    }
    /*
    public void RemoveCardFromInGameCards(GameObject newCard)
    {
        Debug.Log("trying to remove card from list with seed " + newCard.GetComponent<InGameCard>().cardData.seed);
        if(inGameCards.ContainsValue(newCard))
        {
            Debug.LogWarning("removing " + newCard.GetComponent<InGameCard>().cardData.cardName + " from list");
            inGameCards.Remove(newCard.GetComponent<InGameCard>().cardData.seed);
        }
    }
    */
    public void RemoveCardFromInGameCards(string seed)
    {
        if (inGameCards.ContainsKey(seed))
        {
            Debug.LogWarning("removing " + seed + " from list");
            inGameCards.Remove(seed);
        }
    }
    public GameObject GetCardFromInGameCards(string seed)
    {
        if(inGameCards.ContainsKey(seed))
        {
            return inGameCards[seed];
        }
        else
        {
            Debug.LogError("card with seed was not found from list: " + seed);
            return null;
        }
        
    }

    public bool CheckIfInGameCardsContainsCard(GameObject card)
    {
        if(inGameCards.ContainsValue(card))
        {
            return true;
        }
        return false;
    }

    [Button] public void PrintInGameCards()
    {
        Debug.LogWarning("Cards in inGameCards: " + inGameCards.Count + " cards: ");

        foreach (KeyValuePair<string, GameObject> kvp in inGameCards)
        {
            Debug.Log(kvp.Key + " " + kvp.Value.GetComponent<InGameCard>().GetData().cardName);
        }
            

    }



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
        if (burnCardMessage.denied)
        {
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue - GetCardFromInGameCards(burnCardMessage.seed).GetComponent<InGameCard>().GetData().value);
            ReturnBurnedCardToHand(burnCardMessage.seed);
            return;
        }


        burnCardMessage.burnedCardDone = JsonUtility.FromJson<DrawCardMessage>(burnCardMessage.burnedCard);
        DrawCardMessage cardMessage = burnCardMessage.burnedCardDone;
        if (cardMessage.player == playerNumber)
        {
            playerStats.playerHandCards--;
        }
        else
        {
            GameObject newCard;
            Vector3 cardPos = References.i.opponentBonfire.transform.position;
            cardPos.z = Hand.Instance.gameObject.transform.position.z;
            newCard = Instantiate(References.i.fieldCard, cardPos, Quaternion.Euler(0, 180, 0));
            EnemyHand.Instance.RemoveCard(burnCardMessage.seed);
            CardData cardData = References.i.cardList.GetCardData(cardMessage);
            newCard.GetComponent<InGameCard>().SetNewCardData(false, cardData);
            enemyPlayerStats.playerBurnValue = burnCardMessage.newBurnValue;
            PlayerBurnCard(newCard, cardMessage.player);
        }
    }
    public void PlayerModifyHealth(ModifyHealth modifyHealth)
    {
        if (debuggerModeOn) Debug.Log("player modifyHealth " + modifyHealth.amount);
        if(IsYou(modifyHealth.player))
        {
            if(modifyHealth.amount > 0)
            {
                playerStats.playerHealth += modifyHealth.amount;
            }
            else if(modifyHealth.amount < 0)
            {
                playerStats.playerHealth -= modifyHealth.amount;
            }
        }
        else
        {
            if (modifyHealth.amount > 0)
            {
                enemyPlayerStats.playerHealth += modifyHealth.amount;
            }
            else if(modifyHealth.amount < 0)
            {
                enemyPlayerStats.playerHealth -= modifyHealth.amount;
            }
        }
    }

    public void ReturnBurnedCardToHand(string seed)
    {
        if (debugPlayerBurnCard) Debug.Log("Ret removing ");
        Hand.AddNewCardToHand(GetCardFromInGameCards(seed));
    }

    public void PlayerBurnCard(GameObject card, int player = -1)
    {
        if (player == -1) player = playerNumber;
        sfxLibrary.GetComponent<BurnSFX>().Play();
        
        int value = card.GetComponent<InGameCard>().GetData().value;
        if (player == playerNumber)
        {
            References.i.yourBonfire.GetComponent<Bonfire>().PlayEffect();
            if(debugPlayerBurnCard) Debug.Log("adding ");
            card.transform.SetParent(References.i.yourBonfire.transform);
            UpdatePlayerBurnValue(player, playerStats.playerBurnValue + value);
            References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            References.i.opponentBonfire.GetComponent<Bonfire>().PlayEffect();
            card.transform.SetParent(References.i.opponentBonfire.transform);
            References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
        }
        card.GetComponent<InGameCard>().Burn();
    }
    public void UpdatePlayerBurnValue(int player, int newValue)
    {
        if(player == playerNumber)
        {   
            playerStats.playerBurnValue = newValue;
            References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue = newValue;
            References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
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
                PlayerDrawCard(drawCardMessage.player, drawCardMessage.seed);
                GameManager.Instance.playerStats.playerHandCards++;
            }
            else
            {
                if(References.i.yourDeckObj.GetComponent<Deck>() != null) {
                    References.i.yourDeckObj.GetComponent<Deck>().StartDrawCooldown(drawCardMessage.drawCooldown);
                } else if (References.i.yourDeckObj.GetComponent<TutorialDeck>() != null) {
                    References.i.yourDeckObj.GetComponent<TutorialDeck>().StartDrawCooldown(drawCardMessage.drawCooldown);
                }
                
            }
            AddCardToInGameCards(Hand.RevealNewCard(drawCardMessage));

        }
        else
        {
            PlayerDrawCard(drawCardMessage.player, drawCardMessage.seed);
        }
    }

    public void AddNewCard(AddNewCardMessage addNewCardMessage)
    {
        Debug.Log("Player " + addNewCardMessage.player + " added new card to somewhere");
    }

    public void EnchantmentEffectActive(EnchantmentEffectMesage enchantmentEffect)
    {
        enchantmentEffect.cardInfo = JsonUtility.FromJson<DrawCardMessage>(enchantmentEffect.card);
        bool isYou = false;
        if (IsYou(enchantmentEffect.cardInfo.player)) isYou = true;
        References.i.cardEnchantmentEffectManager.PlayEnchantmentEffect((Enchantment.Trigger)enchantmentEffect.trigger, enchantmentEffect.seed, isYou);
        References.i.cardEnchantmentEffectManager.DisplayCardEffectSource(References.i.cardList.GetCardData(enchantmentEffect.cardInfo));
		sfxLibrary.transform.GetComponent<EffectActivationSFX>().Play();
    }


    public void PlayerDrawCard(int player, string seed = "")
    {
        if(player == playerNumber)
        {
            playerStats.deckCardCount--;
            Hand.AddNewCard();
        }
        else
        {
            enemyPlayerStats.deckCardCount--;
            if(debugPlayerDrawCard) Debug.Log("Remaining cards " + enemyPlayerStats.deckCardCount);
            EnemyHand.AddNewCard(seed);
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
    public void PlayerSummonCard(SummonCardMessage summonCardMessage)
    {
        if (summonCardMessage.player == playerNumber)
        {
            if(Hand.Instance.GetVisibleCardWithSeed(summonCardMessage.seed) != null)
            {
                Hand.Instance.TryRemoveCard(summonCardMessage.seed);
            }

            if(summonCardMessage.auto)
            {
                AddCardToInGameCards(References.i.yourMonsterZone.AutoAddNewMonsterCard(false, summonCardMessage.boardIndex, References.i.cardList.GetCardData(summonCardMessage)));
                References.i.yourMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(summonCardMessage.attackCooldown, true);
                playerStats.playerFieldCards++;
                playerStats.playerHandCards--;
            }
            else
            {
                AddCardToInGameCards(References.i.yourMonsterZone.UpdateCardData(true, summonCardMessage));
                References.i.yourMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(summonCardMessage.attackCooldown, true);
                playerStats.playerHandCards--;
            }
            References.i.yourMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().owner = summonCardMessage.player;
            
        }
        else
        {
            if (EnemyHand.Instance.GetCardWithSeed(summonCardMessage.seed) != null)
            {
                EnemyHand.Instance.RemoveCard(summonCardMessage.seed);
            }

            if (!summonCardMessage.auto)
            {
                enemyPlayerStats.playerBurnValue -= summonCardMessage.cardCost;
                References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
            }

            References.i.opponentMonsterZone.AddNewMonsterCard(false, summonCardMessage.boardIndex, References.i.cardList.GetCardData(summonCardMessage));
            References.i.opponentMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(summonCardMessage.attackCooldown, true);
            References.i.opponentMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().owner = summonCardMessage.player;
            //enemyPlayerStats.playerFieldCards++;
        }
        
    }

    public void RemoveCard(RemoveCardMessage removeCardMessage)
    {
        
        if (IsYou(removeCardMessage.player))
        {
            if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.Hand)
            {
                if (removeCardMessage.removal)
                {
                    Hand.Instance.TryRemoveCard(removeCardMessage.seed);
                }
                playerStats.playerHandCards--;
            }
            else if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.Deck)
            {
                playerStats.deckCardCount--;
            }
            else if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.DiscardPile)
            {
                playerStats.discardpileCardCount--;
            }
        }
        else
        {
            if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.Hand)
            {
                //Add remove card from in game cards here
                EnemyHand.Instance.RemoveCard(removeCardMessage.seed);
                enemyPlayerStats.playerHandCards--;
            }
            else if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.Deck)
            {
                enemyPlayerStats.deckCardCount--;
            }
            else if ((RemoveCardMessage.CardSource)removeCardMessage.source == RemoveCardMessage.CardSource.DiscardPile)
            {
                enemyPlayerStats.discardpileCardCount--;
            }
        }
    }

    public void CardSummonDenied()
    {
        playerStats.playerFieldCards--;
        UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue + References.i.yourMonsterZone.unhandledCards[0].GetComponent<InGameCard>().GetData().cost);
        References.i.yourMonsterZone.RecallCard(playerNumber, References.i.yourMonsterZone.unhandledCards[0]);
        Hand.Instance.UpdateCanAffortCards();
    }


    public void PlayerPlayCard(PlayCardMessage playCardMessage)
    {
        if(playCardMessage.denied)
        {
            playerStats.playerFieldCards--;
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue + References.i.yourMonsterZone.unhandledCards[0].GetComponent<InGameCard>().GetData().cost);
            if (playCardMessage.serverBurnValue != playerStats.playerBurnValue) UpdatePlayerBurnValue(playerNumber, playCardMessage.serverBurnValue);
            References.i.yourMonsterZone.RecallCard(playerNumber, References.i.yourMonsterZone.unhandledCards[0]);
            return;
        }
        
        if (playCardMessage.player == playerNumber)
        {
         
            if(playCardMessage.auto)
            {
                References.i.yourMonsterZone.AutoAddNewMonsterCard(true, playCardMessage.boardIndex, References.i.cardList.GetCardData(playCardMessage));
                Hand.Instance.TryRemoveCard(playCardMessage.seed);
                References.i.yourMonsterZone.GetCardWithSeed(playCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(playCardMessage.attackCooldown, true);
                return;
            }

            References.i.yourMonsterZone.UpdateCardData(true, playCardMessage);
            References.i.yourMonsterZone.GetCardWithSeed(playCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(playCardMessage.attackCooldown, true);
            
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

            Debug.Log("player: " + playCardMessage.player);
            References.i.yourMonsterZone.GetCardWithSeed(playCardMessage.seed).GetComponent<InGameCard>().owner = playCardMessage.player;
        }
        else
        {
            PlayerPlayCard(References.i.cardList.GetCardData(playCardMessage), playCardMessage.boardIndex, playCardMessage.player, playCardMessage.attackCooldown);
        }
    }
    public void PrePlayCard(CardData data, bool hasTargetAbility = false)
    {
        if(data.cardType == Card.CardType.Monster)
        {
            GameObject newCard = References.i.yourMonsterZone.AddNewMonsterCard(true, 0, data);

            //TODO: make sure that card with that seed still exists in the game(instead of removing move to container or something)
            Hand.Instance.TryRemoveCard(data.seed);

            if (!hasTargetAbility)
            {
                PlayerPlayCard(data);
            }
            else
            {
                targettingCardData = data;
                StartTargetEvent(newCard);
            }
            Hand.Instance.UpdateCanAffortCards();
        }
    }

    public void StartTargetEvent(GameObject source)
    {
        Mouse.Instance.targetModeOn = true;
        activeTargetLine = LineRendererManager.Instance.CreateNewLine(source, Mouse.Instance.gameObject);
        
    }
    public void EndTargetEvent(string seed)
    {
        Mouse.Instance.targetModeOn = false;
        activeTargetLine.RemoveLine();
        WebSocketService.PlayCard(References.i.yourMonsterZone.monsterCards.IndexOf(References.i.yourMonsterZone.GetCardWithSeed(targettingCardData.seed)), targettingCardData.seed, seed);
        PlayerPlayCard(targettingCardData);
    }


    public void PlayerPlayCard(CardData data, int boardIndex = 0, int player = -1, float attackCD = 0)
    {
        if (player == -1) player = playerNumber;
        if(player == playerNumber)
        {
            playerStats.playerBurnValue -= data.cost;
            References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = playerStats.playerBurnValue.ToString();
        }
        else
        {
            enemyPlayerStats.playerBurnValue -= data.cost;
            References.i.opponentMonsterZone.GetCardWithSeed(data.seed).GetComponent<InGameCard>().StartAttackCooldown(attackCD, true);
            References.i.opponentMonsterZone.GetCardWithSeed(data.seed).GetComponent<InGameCard>().owner = player;
            //for now removes card with index 0
            if(debugPlayerPlayCard) Debug.Log("Removing card from hand");
            EnemyHand.Instance.RemoveCard(data.seed);
            References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
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
            if (debugPlayerAttack) Debug.Log("attacked seed: " + attackEventMessage.attackerValues.seed + " target seed: " + attackEventMessage.targetValues.seed + "attacker lp: " + attackEventMessage.attackerValues.lp + " rp: " + attackEventMessage.attackerValues.rp + " target lp: " + attackEventMessage.targetValues.lp + " rp: " + attackEventMessage.targetValues.rp);
            bool wasYourAttack = false;
            if (attackEventMessage.player == playerNumber) wasYourAttack = true;
            if (debugPlayerAttack) Debug.LogWarning("was your attack " + wasYourAttack);
            References.i.attackEventHandler.StartAttackEvent(wasYourAttack, attacker, target, attackEventMessage.attackCooldown);
        }
    }

    public void CardDataChange(StatChangeMessage statChangeMessage)
    {
        Debug.Log("message: " + JsonUtility.ToJson(statChangeMessage));

        List<CardDataMessage> finalCardDatas = new List<CardDataMessage>();
        foreach(string target in statChangeMessage.targets)
        {
            CardDataMessage data = JsonUtility.FromJson<CardDataMessage>(target);
            finalCardDatas.Add(data);
        }
        statChangeMessage.convertedTargets = finalCardDatas;

        foreach(CardDataMessage cardData in statChangeMessage.convertedTargets)
        {
            GameObject target = GetCardFromInGameCards(cardData.seed);
            CardData data = target.GetComponent<InGameCard>().GetData();

            data.rp = cardData.rp;
            data.lp = cardData.lp;

            if (data.lp <= 0 || data.rp <= 0)
            {
                GameManager.Instance.playerStats.playerFieldCards--;
                References.i.yourMonsterZone.TryRemoveMonsterCard(cardData.seed);
                References.i.opponentMonsterZone.TryRemoveMonsterCard(cardData.seed);
            }


            data.enchantments = cardData.enchantments;
            data.monsterTags = cardData.mtag;
            data.spellTags = cardData.stag;
            data.cardName = cardData.cardName;
            data.cost = cardData.cardCost;
            data.attackDirection = (Card.AttackDirection)cardData.attackDirection;
            data.value = cardData.cardValue;
            PlayDataChangeEffect(statChangeMessage.buffType, target);
            target.GetComponent<InGameCard>().UpdateCardTexts();
        }
    }

    public void PlaySpell(PlaySpellMessage playSpellMessage)
    {
        if (IsYou(playSpellMessage.player))
        {
            References.i.spellZone.PlaySpell(playSpellMessage.seed, playSpellMessage.targets, playSpellMessage.windup);
            GameManager.Instance.playerStats.playerHandCards--;
        }
        else
        {
            enemyPlayerStats.playerBurnValue -= playSpellMessage.cardCost;
            GameManager.Instance.enemyPlayerStats.playerHandCards--;
            References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
            References.i.spellZone.PlaySpell(References.i.cardList.GetCardData(playSpellMessage), playSpellMessage.targets, playSpellMessage.windup);
        }
    }
    public void TriggerSpell(TriggerSpellMessage triggerSpellMessage)
    {
        SpellZone.Instance.TriggerSpellChain(triggerSpellMessage.index, triggerSpellMessage.denied);
    }

    public void PlayDataChangeEffect(CardDataMessage.BuffType buffType, GameObject target)
    {
        switch(buffType)
        {
            case CardDataMessage.BuffType.Damage:
                Debug.LogWarning("Trigger Damage effect here");
                break;
            case CardDataMessage.BuffType.StatBuff:
                target.GetComponent<InGameCard>().PlayBuffEffect();
                Debug.LogWarning("Trigger StatBuff effect here");
                break;
            case CardDataMessage.BuffType.StatDebuff:
                Debug.LogWarning("Trigger StatDebuff effect here");
                break;
            case CardDataMessage.BuffType.Default:
                Debug.LogWarning("Trigger Default effect here");
                break;
        }
    }


    public bool IsYou(int player)
    {
        if (player == playerNumber) return true;
        else return false;
    }
    public void AttackDenied(CardPowersMessage attacker)
    {
        References.i.yourMonsterZone.GetCardWithSeed(attacker.seed).GetComponent<InGameCard>().StartAttackCooldown(0);
    }
}
