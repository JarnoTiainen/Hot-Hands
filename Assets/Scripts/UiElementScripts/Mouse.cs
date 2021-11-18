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
    [SerializeField] public float mouseHightFromTableTop;
    [SerializeField] public bool targetModeOn;
    [SerializeField] public GameObject targetSource;
    [SerializeField] private GameObject spellSwirlPrefab;

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
                    TransformIntoTargetMode();
                    if (Input.GetMouseButtonUp(0))
                    {
                        TryCastSpell();
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
            TryBurnCard();
        }
        else
        {
            References.i.yourMonsterZone.RemoveGhostCard();
            Hand.Instance.ReturnVisibleCard(heldCard);
            heldCard = null;
        }
    }
    public void TrySummonMonster()
    {
        int playerBurnValue = GameManager.Instance.playerStats.playerBurnValue;
        bool canAffordToPlayCard = playerBurnValue >= heldCard.GetComponent<InGameCard>().GetData().cost;
        bool isEnoghSpace = GameManager.Instance.playerStats.playerFieldCards < GameManager.Instance.maxFieldCardCount;

        if (canAffordToPlayCard && isEnoghSpace)
        {
            PlayCard();
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
        bool isEnoughSpace = true; //TODO: change this to check if there are open spellSlots

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
        References.i.yourMonsterZone.RemoveGhostCard();
        if (debuggingModeOn) Debug.Log("Card discarded with seed: " + heldCard.GetComponent<InGameCard>().GetData().seed);
        if (GameManager.Instance.GetCardFromInGameCards(heldCard.GetComponent<InGameCard>().GetData().seed) != null)
        {
            WebSocketService.Burn(heldCard.GetComponent<InGameCard>().GetData().seed);
            GameManager.Instance.PlayerBurnCard(heldCard);
            Hand.Instance.RemoveCardNoDestroy(heldCard.GetComponent<InGameCard>().GetData().seed);
            heldCard = null;
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
    }
    public void TransformIntoTargetMode()
    {
        heldCard.GetComponent<HandCard>().SwitchToSpellMode();
        
    }
    public void TransformIntoCardMode()
    {
        heldCard.GetComponent<HandCard>().SwitchToCardMode();
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
                WebSocketService.PlayCard(References.i.yourMonsterZone.monsterCards.IndexOf(References.i.yourMonsterZone.ghostCard), heldCard.GetComponent<InGameCard>().GetData().seed);
                GameManager.Instance.PrePlayCard(heldCard.GetComponent<InGameCard>().GetData(), false);
            }
            
        }
        else
        {
            if (heldCard.GetComponent<InGameCard>().GetData().targetting)
            {
                if(RayCaster.Instance.target)
                {
                    if (RayCaster.Instance.target.GetComponent<InGameCard>())
                    {
                        if(GameManager.Instance.CheckIfInGameCardsContainsCard(RayCaster.Instance.target))
                        {
                            GameManager.Instance.playerStats.playerBurnValue -= heldCard.GetComponent<InGameCard>().GetData().cost;
                            References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = GameManager.Instance.playerStats.playerBurnValue.ToString();
                            WebSocketService.PlayCard(0, heldCard.GetComponent<InGameCard>().GetData().seed, RayCaster.Instance.target.GetComponent<InGameCard>().GetData().seed);
                            LimboCardHolder.Instance.StoreNewCard(heldCard);
                            heldCard.GetComponent<InGameCard>().isInHand = false;
                            TransformIntoCardMode();
                            heldCard = null;
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
                WebSocketService.PlayCard(0, heldCard.GetComponent<InGameCard>().GetData().seed);
                LimboCardHolder.Instance.StoreNewCard(heldCard);
                TransformIntoCardMode();
                heldCard = null;
            }

        }
    }
}
