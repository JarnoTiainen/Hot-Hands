using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerStats playerStats;
    public PlayerStats enemyPlayerStats;
    public float attackDuration = 0.3f;
    public float moveDuration = 0.4f;
    public float rotationSpeed = 0.2f;
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
    private GameObject targettingCard;

    [SerializeField] private Dictionary<string, GameObject> inGameCards = new Dictionary<string, GameObject>();

    public void SetOpponentName(string name)
    {
        PlayerNamesUIScript.Instance.SetOpponentName(name);
    }

    public void ResetPlayerStats()
    {
        playerStats = new PlayerStats(playerStartHealth);
        enemyPlayerStats = new PlayerStats(playerStartHealth);
    }

    public void EndGame(int player)
    {
        if(IsYou(player))
        {
            MatchResultScript.Instance.GameEnd(true);
        }
        else
        {
            MatchResultScript.Instance.GameEnd(false);
        }
    }

    public void LoadChat(LoadChatMessage loadChatMessage)
    {
        List<Message> newparsedMessages = new List<Message>();
        foreach(string rawMessage in loadChatMessage.rawMessages)
        {
            newparsedMessages.Add(JsonUtility.FromJson<Message>(rawMessage));
        }
        ChatManager.Instance.PopulateChat(newparsedMessages);
    }


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

    public void PlayerBurnCard(BurnCardMessage burnCardMessage)
    {
        if (burnCardMessage.denied)
        {
            UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue - GetCardFromInGameCards(burnCardMessage.seed).GetComponent<InGameCard>().GetData().value);
            ReturnBurnedCardToHand(burnCardMessage.seed);
            playerStats.discardpileCardCount--;
            return;
        }

        burnCardMessage.burnedCardDone = JsonUtility.FromJson<DrawCardMessage>(burnCardMessage.burnedCard);
        DrawCardMessage cardMessage = burnCardMessage.burnedCardDone;
        if (cardMessage.player == playerNumber)
        {
            playerStats.playerHandCards--;
            playerStats.discardpileCardCount++;
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
        Debug.Log("player modifyHealth " + modifyHealth.amount + " player hp: " + playerStats.playerHealth);
        if(IsYou(modifyHealth.player))
        {
            if(modifyHealth.amount > 0)
            {
                playerStats.playerHealth += modifyHealth.amount;
            }
            else if(modifyHealth.amount < 0)
            {
                playerStats.playerHealth += modifyHealth.amount;
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
                enemyPlayerStats.playerHealth += modifyHealth.amount;
            }
        }
        Debug.Log("player modifyHealth " + modifyHealth.amount + " player hp: " + playerStats.playerHealth);
    }

    public void ReturnBurnedCardToHand(string seed)
    {
        if (debugPlayerBurnCard) Debug.Log("Ret removing ");
        Hand.AddNewCardToHand(GetCardFromInGameCards(seed));
    }

    public void PlayerBurnCard(GameObject card, int player = -1)
    {
        if (player == -1) player = playerNumber;
        SFXLibrary.Instance.burnCard.PlaySFX();
        
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
            Hand.Instance.UpdateCanAffortCards();
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
            if (drawCardMessage.drawCooldown == -1)
            {
                if(drawCardMessage.enemyDeck)
                {
                    Hand.AddNewCardFromOpponentDeck();
                }
                else
                {
                    PlayerDrawCard(drawCardMessage.player, drawCardMessage.seed);
                }
                
                GameManager.Instance.playerStats.playerHandCards++;
            }
            else
            {
                if(References.i.yourDeckObj.GetComponent<Deck>() != null) {
                    References.i.yourDeck.GetComponent<DeckGaObConstructor>().StartDrawCooldown(drawCardMessage.drawCooldown);
                } else if (References.i.yourDeckObj.GetComponent<TutorialDeck>() != null) {
                    References.i.yourDeckObj.GetComponent<TutorialDeck>().StartDrawCooldown(drawCardMessage.drawCooldown);
                }
                
            }
            AddCardToInGameCards(Hand.RevealNewCard(drawCardMessage));

        }
        else
        {
            
            if (!References.i.mouse.tutorialMode) {
                if (drawCardMessage.enemyDeck)
                {
                    enemyPlayerStats.playerHandCards++;
                    playerStats.deckCardCount--;
                    EnemyHand.AddNewCardFromEnemyDeck(drawCardMessage.seed);
                }
                else
                {
                    PlayerDrawCard(drawCardMessage.player, drawCardMessage.seed);
                    enemyPlayerStats.playerHandCards++;
                }
                    
            } 
            else 
            {
                if (drawCardMessage.enemyDeck)
                {
                    enemyPlayerStats.playerHandCards++;
                    playerStats.deckCardCount--;
                    EnemyHand.AddNewCardFromEnemyDeck(drawCardMessage.seed);
                }
                else
                {
                    PlayerDrawCard(drawCardMessage.player, drawCardMessage.seed, drawCardMessage);
                    enemyPlayerStats.playerHandCards++;
                }
                    
            }
        }
    }

    public void PlayerDrawCard(int player, string seed = "", DrawCardMessage drawCardMessage = null)
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
            if (drawCardMessage == null) {
                EnemyHand.AddNewCard(seed);
            } else {
                EnemyHand.AddNewCard(seed, drawCardMessage);
            }
            
        }
        SFXLibrary.Instance.CardDraw();
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
		SFXLibrary.Instance.effectActivation.PlaySFX();
    }


    

    [Button]public void PlayerReturnDrawCard()
    {
        Hand.RemoveHiddenCard();
        References.i.yourDeck.GetComponent<DeckGaObConstructor>().AddSingleCardToDeck();
        References.i.yourDeck.GetComponent<DeckGaObConstructor>().StartDrawCooldown(0);
        playerStats.deckCardCount++;
        playerStats.playerHandCards--;
    }

    public void SetDeck(SetDeckMessage setDeckMessage)
    {
        if(setDeckMessage.player == playerNumber)
        {
            playerStats.deckCardCount = setDeckMessage.deckCards;
            playerStats.discardpileCardCount = 0;
            References.i.yourDeck.GetComponent<DeckGaObConstructor>().CreateDeck();
            deckSet = true;
        }
        else
        {
            enemyPlayerStats.deckCardCount = setDeckMessage.deckCards;
            enemyPlayerStats.discardpileCardCount = 0;
            References.i.opponentDeck.GetComponent<DeckGaObConstructor>().CreateDeck();
        }
    }
    public void PlayerSummonCard(SummonCardMessage summonCardMessage)
    {
        Debug.Log("attack cd gamemanager " + summonCardMessage.attackCooldown);
        if (summonCardMessage.player == playerNumber)
        {
            if(Hand.Instance.GetVisibleCardWithSeed(summonCardMessage.seed) != null)
            {
                Hand.Instance.TryRemoveCard(summonCardMessage.seed);
            }

            if(summonCardMessage.auto)
            {
                AddCardToInGameCards(References.i.yourMonsterZone.AutoAddNewMonsterCard(true, summonCardMessage.boardIndex, References.i.cardList.GetCardData(summonCardMessage)));
                References.i.yourMonsterZone.GetCardWithSeed(summonCardMessage.seed).GetComponent<InGameCard>().StartAttackCooldown(summonCardMessage.attackCooldown, true);
                playerStats.playerFieldCards++;
                if(summonCardMessage.cardSource == SummonCardMessage.CardSource.Hand)
                {
                    playerStats.playerHandCards--;
                }
                else if (summonCardMessage.cardSource == SummonCardMessage.CardSource.Deck)
                {
                    playerStats.deckCardCount--;
                }
                else if(summonCardMessage.cardSource == SummonCardMessage.CardSource.Void)
                {

                }
                else if(summonCardMessage.cardSource == SummonCardMessage.CardSource.DiscardPile)
                {
                    playerStats.discardpileCardCount--;
                }

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
        if(References.i.yourMonsterZone.unhandledCards.Count > 0)
        UpdatePlayerBurnValue(playerNumber, playerStats.playerBurnValue + References.i.yourMonsterZone.unhandledCards[0].GetComponent<InGameCard>().GetData().cost);
        References.i.yourMonsterZone.RecallCard(playerNumber, References.i.yourMonsterZone.unhandledCards[0]);
        Hand.Instance.UpdateCanAffortCards();
    }
    [Button] public void CardDenied(string seed)
    {
        if(References.i.yourMonsterZone.monsterCards.Contains(GetCardFromInGameCards(seed)))
        {
            References.i.yourMonsterZone.TryReturnCardToHand(seed);
        }
        else
        {
            Hand.AddNewCardToHand(GetCardFromInGameCards(seed));
        }
    }


    public void PlayerPlayCard(PlayCardMessage playCardMessage)
    {
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
        SFXLibrary.Instance.CardPlay();
    }

    public void PrePlayCard(CardData data, bool hasTargetAbility = false)
    {
        if(data.cardType == Card.CardType.Monster)
        {
            GameObject newCard = References.i.yourMonsterZone.AddNewMonsterCard(true, 0, data);
            GameManager.Instance.AddCardToInGameCards(newCard);
            //TODO: make sure that card with that seed still exists in the game(instead of removing move to container or something)
            Hand.Instance.TryRemoveCard(data.seed);

            if (!hasTargetAbility)
            {
                PlayerPlayCard(data);
            }
            else
            {
                targettingCardData = data;
                targettingCard = newCard;
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
    public void CancelTargetEvent()
    {
        Debug.Log("cancelling");

        Mouse.Instance.targetModeOn = false;
        activeTargetLine.RemoveLine();
        References.i.yourMonsterZone.TryReturnCardToHand(targettingCardData.seed);

    }

    public void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(targettingCardData != null)
            {
                if (Mouse.Instance.targetModeOn)
                {
                    if(RayCaster.Instance.target.GetComponent<InGameCard>())
                    {
                        if(!IsYou(RayCaster.Instance.target.GetComponent<InGameCard>().owner) && targettingCardData.targetType == Enchantment.TargetType.Ally)
                        {
                            CancelTargetEvent();
                            return;
                        }
                        if (IsYou(RayCaster.Instance.target.GetComponent<InGameCard>().owner) && targettingCardData.targetType == Enchantment.TargetType.Enemy)
                        {
                            CancelTargetEvent();
                            return;
                        }
                        GameManager.Instance.EndTargetEvent(RayCaster.Instance.target.GetComponent<InGameCard>().GetData().seed);
                    }
                    if (!RayCaster.Instance.target.GetComponent<InGameCard>())
                    {
                        CancelTargetEvent();
                    }
                }
            }
            
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            playerStats.playerFieldCards = References.i.yourMonsterZone.monsterCards.Count;
            if (References.i.yourMonsterZone.ghostCard != null) playerStats.playerFieldCards--;
        }

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

    public void TutorialPlayerAttack(AttackEventMessage attackEventMessage)
    {
        
        if (attackEventMessage.directHit)
        {
            if (attackEventMessage.player == playerNumber)
            {
                References.i.attackEventHandler.StartAttackEvent(true, attackEventMessage.attackerValues, attackEventMessage.playerTakenDamage, attackEventMessage.attackCooldown);
            }
            else
            {    
                References.i.attackEventHandler.StartAttackEvent(false, attackEventMessage.attackerValues, attackEventMessage.playerTakenDamage, attackEventMessage.attackCooldown);
            }
        }
        else
        {
            bool wasYourAttack = false;
            if (attackEventMessage.player == playerNumber) wasYourAttack = true;
            References.i.attackEventHandler.StartAttackEvent(wasYourAttack, attackEventMessage.attackerValues, attackEventMessage.targetValues, attackEventMessage.attackCooldown);
        }
        Debug.Log("Player taken dmg " + attackEventMessage.playerTakenDamage);
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
            Debug.Log("seed " + JsonUtility.ToJson(cardData));
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
            data.cardName = cardData.name;
            data.cost = cardData.cost;
            data.attackDirection = (Card.AttackDirection)cardData.direction;
            data.value = cardData.value;
            Debug.Log("card new cost = " + cardData.cost);
            PlayDataChangeEffect(statChangeMessage.buffType, target);
            target.GetComponent<InGameCard>().SetNewDescription(References.i.cardList.GetCardDescription(data.enchantments));
            target.GetComponent<InGameCard>().UpdateCardTexts();
            target.GetComponent<InGameCard>().SetDescription();
        }
    }

    public void PlaySpell(PlaySpellMessage playSpellMessage)
    {
        
        if (IsYou(playSpellMessage.player))
        {
            References.i.spellZone.PlaySpell(playSpellMessage.seed, playSpellMessage.targets, playSpellMessage.windup, playSpellMessage.slot);
            playerStats.playerHandCards--;
            playerStats.discardpileCardCount++;
            if (References.i.mouse.tutorialMode) {
                if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.PlaySpell) {
                    if (!TutorialManager.tutorialManagerInstance.firstSpell) {
                        TutorialManager.tutorialManagerInstance.firstSpell = true;
                    }
                    TutorialManager.tutorialManagerInstance.NextTutorialState();
                }
            }
        }
        else
        {
            enemyPlayerStats.playerBurnValue -= playSpellMessage.cardCost;
            enemyPlayerStats.playerHandCards--;
            enemyPlayerStats.discardpileCardCount++;
            References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = enemyPlayerStats.playerBurnValue.ToString();
            GameObject card = GetCardFromInGameCards(playSpellMessage.seed);
            Debug.Log("card naem: " + card.name);
            card.GetComponent<InGameCard>().SetNewCardData(false, References.i.cardList.GetCardData(playSpellMessage));
            AddCardToInGameCards(card);
            EnemyHand.Instance.RemoveCard(playSpellMessage.seed);
            References.i.spellZone.PlaySpell(References.i.cardList.GetCardData(playSpellMessage), playSpellMessage.targets, playSpellMessage.windup, playSpellMessage.slot);

            if (References.i.mouse.tutorialMode) {
                if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.PlaySpell) {
                    TutorialManager.tutorialManagerInstance.NextTutorialState();
                }
            }
        }


    }

    public void TriggerSpell(TriggerSpellMessage triggerSpellMessage)
    {
        SpellZone.Instance.TriggerSpellChain(triggerSpellMessage.index, triggerSpellMessage.denied);
    }

    public void LockSpellChain(bool lockState)
    {

    }

    public void PlayDataChangeEffect(CardDataMessage.BuffType buffType, GameObject target)
    {
        switch(buffType)
        {
            case CardDataMessage.BuffType.Damage:
                target.GetComponent<InGameCard>().PlayTakeDamageEffect();
                break;
            case CardDataMessage.BuffType.StatBuff:
                target.GetComponent<InGameCard>().PlayBuffEffect();
                Debug.LogWarning("Trigger StatBuff effect here");
                break;
            case CardDataMessage.BuffType.StatDebuff:
                target.GetComponent<InGameCard>().PlayDebuffEffect();
                Debug.LogWarning("Trigger StatDeBuff effect here");
                break;
            case CardDataMessage.BuffType.Default:
                Debug.LogWarning("Trigger Default effect here");
                break;
        }
        StartCoroutine(DataChangeSFX(buffType));
    }

    private IEnumerator DataChangeSFX(CardDataMessage.BuffType buffType)
    {
        switch (buffType)
        {
            case CardDataMessage.BuffType.Damage:
                yield return null;
                SFXLibrary.Instance.hit.PlaySFX();
                Debug.Log("Damage BuffType, hit SFX");
                break;
            case CardDataMessage.BuffType.StatBuff:
                yield return new WaitForSeconds(0.2f);
                SFXLibrary.Instance.cardBuff.PlaySFX();
                break;
            case CardDataMessage.BuffType.StatDebuff:
                yield return new WaitForSeconds(0.2f);
                SFXLibrary.Instance.cardDebuff.PlaySFX();
                break;
            default:
                yield return null;
                Debug.Log("Default BuffType, hit SFX");
                SFXLibrary.Instance.hit.PlaySFX();
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
