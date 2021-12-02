using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mouse : MonoBehaviour
{
    public static Mouse Instance { get; private set; }


    public Camera mainCam;
    public GameObject heldCard;
    [SerializeField] private Vector2 monsterHitBox;
    public Vector2 mousePosInWorld;
    public GameObject markerPrefab;
    [SerializeField] private bool debuggingModeOn = false;
    [SerializeField] public bool tutorialMode;
    [SerializeField] public float mouseHightFromTableTop;
    [SerializeField] public bool targetModeOn;
    [SerializeField] public GameObject targetSource;
    [SerializeField] private GameObject spellSwirlPrefab;
    private int burnTries;

    private void Awake()
    {

        Instance = gameObject.GetComponent<Mouse>();
    }

    private void Start()
    {
        if(debuggingModeOn)
        {
            Instantiate(markerPrefab, new Vector3(monsterHitBox.x, monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(-monsterHitBox.x, monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(monsterHitBox.x, -monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(-monsterHitBox.x, -monsterHitBox.y, 0), Quaternion.identity);
        }
    }

    private void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePosInWorld = raycastHit.point;
            transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, mouseHightFromTableTop);
        }
        
        if(heldCard)
        {
 
            if (heldCard.GetComponent<InGameCard>().GetData().cardType == Card.CardType.Monster)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (debuggingModeOn) Debug.Log("placing down");
                    ValuatePlaceCard();
                }
            }
            else
            {
                if (!Hand.Instance.CheckIfInsideHandBox(transform.position))
                {
                    if (debuggingModeOn) Debug.Log("placing down");
                    if(RayCaster.Instance.target == References.i.yourBonfire)
                    {
                        TransformIntoCardMode();
                        if (Input.GetMouseButtonUp(0))
                        {
                            TryBurnCard();
                        }
                    }
                    else
                    {
                        TransformIntoTargetMode();
                        if (Input.GetMouseButtonUp(0))
                        {
                            TryCastSpell();
                        }
                    }
                    
                    
                }
                else
                {
                    TransformIntoCardMode();
                    if (Input.GetMouseButtonUp(0))
                    {
                        ReturnHeldCardToHand();
                    }
                }
            }
        } 
        
    }

    [Button] public void SetNewHeldCard(GameObject newCard)
    {
        Cursor.visible = false;
        heldCard = newCard;
        heldCard.GetComponent<InGameCard>().isInHand = true;
        newCard.transform.SetParent(gameObject.transform);
        newCard.transform.localPosition = Vector3.zero;
    }



    //This function should trigger them not execute TODO: move stuff to somewhere else

    public void ValuatePlaceCard()
    {

        if (Mathf.Abs(mousePosInWorld.x) < monsterHitBox.x && Mathf.Abs(mousePosInWorld.y) < monsterHitBox.y && heldCard.GetComponent<InGameCard>().GetData().cardType == Card.CardType.Monster)
        {
            TrySummonMonster();
        }
        else if(RayCaster.Instance.target == References.i.yourBonfire)
        {
            if (!tutorialMode) {
                TryBurnCard();
            } else if (TutorialManager.tutorialManagerInstance.burnignAllowed) {
                TryBurnCard();
            } else {
                References.i.yourMonsterZone.RemoveGhostCard();
                Hand.Instance.ReturnVisibleCard(heldCard);
                heldCard = null;
                Cursor.visible = true;
            }
            
        } 
        else
        {
            References.i.yourMonsterZone.RemoveGhostCard();
            Hand.Instance.ReturnVisibleCard(heldCard);
            heldCard = null;
            Cursor.visible = true;
        }
    }
    public void TrySummonMonster()
    {
        int playerBurnValue = GameManager.Instance.playerStats.playerBurnValue;
        bool canAffordToPlayCard = playerBurnValue >= heldCard.GetComponent<InGameCard>().GetData().cost;
        bool isEnoghSpace = GameManager.Instance.playerStats.playerFieldCards < GameManager.Instance.maxFieldCardCount;

        if (canAffordToPlayCard && isEnoghSpace)
        {
            if (!tutorialMode) {
                PlayCard();
            } else if (TutorialManager.tutorialManagerInstance.summoningAllowed) {
                Debug.Log("tutorial palaycard");
                PlayCard();
            } else {
                ReturnHeldCardToHand();
            }
            
        }
        else
        {
            ReturnHeldCardToHand();
        }
    }
    public void TryCastSpell()
    {
        int playerBurnValue = GameManager.Instance.playerStats.playerBurnValue;
        bool canAffordToPlayCard = playerBurnValue >= heldCard.GetComponent<InGameCard>().GetData().cost;
        bool isEnoughSpace = SpellZone.Instance.HasFreeSlot();

        Debug.Log(playerBurnValue + " >= " + heldCard.GetComponent<InGameCard>().GetData().cost + " " + SpellZone.Instance.HasFreeSlot());

        if (canAffordToPlayCard && isEnoughSpace)
        {
            PlayCard();
        }
        else
        {
            Debug.Log("Could not afford to cast spell");
            TransformIntoCardMode();
            ReturnHeldCardToHand();
        }
    }
    public void TryBurnCard()
    {
        Debug.Log("TryBurnCard");

        References.i.yourMonsterZone.RemoveGhostCard();
        if (debuggingModeOn) Debug.Log("Card discarded with seed: " + heldCard.GetComponent<InGameCard>().GetData().seed);
        if (GameManager.Instance.GetCardFromInGameCards(heldCard.GetComponent<InGameCard>().GetData().seed) != null)
        {
            if (!tutorialMode) {
                WebSocketService.Burn(heldCard.GetComponent<InGameCard>().GetData().seed);
                GameManager.Instance.PlayerBurnCard(heldCard);
                Hand.Instance.RemoveCardNoDestroy(heldCard.GetComponent<InGameCard>().GetData().seed);
                heldCard = null;
                Cursor.visible = true;
            } else if (TutorialManager.tutorialManagerInstance.burnignAllowed) {
                //let the player burn only one specific card
                if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.BurnCard) {
                    Debug.Log("seed " + heldCard.GetComponent<InGameCard>().GetCardData().seed);
                    //don't use magic strings
                    if (heldCard.GetComponent<InGameCard>().GetCardData().seed != "000000018") {
                        burnTries++;
                        Debug.Log("Returning card");
                        ReturnHeldCardToHand();
                        if(burnTries == 5) {
                            //do a funny line here
                        }
                        return;
                    }
                }
                GameManager.Instance.playerStats.playerHandCards--;
                GameManager.Instance.PlayerBurnCard(heldCard);
                Hand.Instance.RemoveCardNoDestroy(heldCard.GetComponent<InGameCard>().GetData().seed);
                heldCard = null;
                Cursor.visible = true;
                if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.BurnCard) {
                    TutorialManager.tutorialManagerInstance.NextTutorialState();
                }
                
            } else {
                Debug.Log("Returning card");
                ReturnHeldCardToHand();
            }
            References.i.yourMonsterZone.RepositionMonsterCards();
        }
        else
        {
            ReturnHeldCardToHand();
        }
    }
    public void ReturnHeldCardToHand()
    {
        References.i.yourMonsterZone.RemoveGhostCard();
        Debug.Log("name. " + heldCard.name);
        Hand.Instance.ReturnVisibleCard(heldCard);
        heldCard.GetComponent<InGameCard>().isInHand = false;
        heldCard = null;
        Cursor.visible = true;
    }
    public void TransformIntoTargetMode()
    {
        heldCard.GetComponent<HandCard>().SwitchToSpellMode();
        
    }
    public void TransformIntoCardMode()
    {
        heldCard.GetComponent<HandCard>().SwitchToCardMode();
        Hand.Instance.UpdateCanAffortCards();
    }

    private void PlayCard()
    {
        if(heldCard.GetComponent<InGameCard>().GetData().cardType == Card.CardType.Monster)
        {
            GameManager.Instance.playerStats.playerFieldCards++;
            if (heldCard.GetComponent<InGameCard>().GetData().targetting)
            {
                Debug.Log("Card was targetting");
                GameManager.Instance.PrePlayCard(heldCard.GetComponent<InGameCard>().GetData(), true);
            }
            else
            {
                if (!tutorialMode) {
                    WebSocketService.PlayCard(References.i.yourMonsterZone.monsterCards.IndexOf(References.i.yourMonsterZone.ghostCard), heldCard.GetComponent<InGameCard>().GetData().seed);
                } 
               
                int ghostIndex = References.i.yourMonsterZone.monsterCards.IndexOf(References.i.yourMonsterZone.ghostCard);
                GameManager.Instance.PrePlayCard(heldCard.GetComponent<InGameCard>().GetData(), false);


                if (tutorialMode) {
                    Debug.Log("tutorial playcard in mouse");
                    CardData cardData = heldCard.GetComponent<InGameCard>().GetData();
                    
                    bool free = cardData.cost == 0;
                    Debug.Log("attack cd mouse " + TutorialManager.tutorialManagerInstance.attackCoolDown);
                    SummonCardMessage summonCard = new SummonCardMessage(ghostIndex, 0, false, free, TutorialManager.tutorialManagerInstance.attackCoolDown, cardData);
                    GameManager.Instance.PlayerSummonCard(summonCard);
                }
            }
            
        }
        else
        {
            //spells
            if (heldCard.GetComponent<InGameCard>().GetData().targetting)
            {
                if(RayCaster.Instance.target)
                {
                    if (RayCaster.Instance.target.GetComponent<InGameCard>())
                    {
                        if(GameManager.Instance.CheckIfInGameCardsContainsCard(RayCaster.Instance.target))
                        {
                            GameManager.Instance.playerStats.playerBurnValue -= heldCard.GetComponent<InGameCard>().GetData().cost;
                            Hand.Instance.UpdateCanAffortCards();
                            References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = GameManager.Instance.playerStats.playerBurnValue.ToString();
                            if (!tutorialMode) {
                                WebSocketService.PlayCard(0, heldCard.GetComponent<InGameCard>().GetData().seed, RayCaster.Instance.target.GetComponent<InGameCard>().GetData().seed);
                            } else {
                                TutorialManager.tutorialManagerInstance.spellCardSeed.Add(heldCard.GetComponent<InGameCard>().GetCardData().seed);
                                PlaySpellMessage playSpellMessage = new PlaySpellMessage(0, heldCard.GetComponent<InGameCard>().GetCardData(), TutorialManager.tutorialManagerInstance.spellWindup, RayCaster.Instance.target.GetComponent<InGameCard>().GetData().seed);
                                GameManager.Instance.PlaySpell(playSpellMessage);
                            }
                            
                            LimboCardHolder.Instance.StoreNewCard(heldCard);
                            heldCard.GetComponent<InGameCard>().isInHand = false;
                            TransformIntoCardMode();
                            heldCard = null;
                            Cursor.visible = true;
                            return;
                        }
                    }
                    else
                    {
                        TransformIntoCardMode();
                        ReturnHeldCardToHand();
                    }
                }
                else
                {
                    TransformIntoCardMode();
                    ReturnHeldCardToHand();
                }
                
            }
            else
            {
                GameManager.Instance.playerStats.playerBurnValue -= heldCard.GetComponent<InGameCard>().GetData().cost;
                
                References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = GameManager.Instance.playerStats.playerBurnValue.ToString();
                if (!tutorialMode) {
                    WebSocketService.PlayCard(0, heldCard.GetComponent<InGameCard>().GetData().seed);
                } else {
                    PlaySpellMessage playSpellMessage = new PlaySpellMessage(0, heldCard.GetComponent<InGameCard>().GetCardData(), TutorialManager.tutorialManagerInstance.spellWindup);
                    GameManager.Instance.PlaySpell(playSpellMessage);
                }
                
                LimboCardHolder.Instance.StoreNewCard(heldCard);
                TransformIntoCardMode();
                heldCard = null;
                
            }

        }
        Cursor.visible = true;
    }
}
