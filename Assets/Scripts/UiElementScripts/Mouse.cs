using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mouse : MonoBehaviour
{
    public static Mouse Instance { get; private set; }

    public MonsterZone yourMonsterZone;
    public GameObject heldCard;
    private int handIndex;
    public Camera thisCamera;
    [SerializeField] private Vector2 monsterHitBox;
    public Vector2 mousePosInWorld;
    public GameObject markerPrefab;
    private Hand uiHand;
    [SerializeField] private bool debuggingOn = false;
    [SerializeField] private float mouseHightFromTableTop;

    private void Awake()
    {
        Instance = gameObject.GetComponent<Mouse>();
        uiHand = GameObject.Find("Hand").GetComponent<Hand>();
    }

    private void Start()
    {
        if(debuggingOn)
        {
            Instantiate(markerPrefab, new Vector3(monsterHitBox.x, monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(-monsterHitBox.x, monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(monsterHitBox.x, -monsterHitBox.y, 0), Quaternion.identity);
            Instantiate(markerPrefab, new Vector3(-monsterHitBox.x, -monsterHitBox.y, 0), Quaternion.identity);
        }
    }

    private void Update()
    {
        Ray ray = thisCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePosInWorld = raycastHit.point;
            transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, mouseHightFromTableTop);
        }
        if(Input.GetMouseButtonUp(0) && heldCard)
        {
            Debug.Log("placing down");
            ValuatePlaceCard();
        }
    }

    [Button] public void SetNewHeldCard(GameObject newCard, int handIndex)
    {
        this.handIndex = handIndex;
        heldCard = newCard;
        newCard.transform.SetParent(gameObject.transform);
        newCard.transform.localPosition = Vector3.zero;
    }



    //This function should trigger them not execute TODO: move stuff to somewhere else

    public void ValuatePlaceCard()
    {

        if (Mathf.Abs(mousePosInWorld.x) < monsterHitBox.x && Mathf.Abs(mousePosInWorld.y) < monsterHitBox.y)
        {

            if (GameManager.Instance.playerStats.playerBurnValue >= Hand.Instance.GetCardData(handIndex).cost)
            {
                GameManager.Instance.playerStats.playerBurnValue -= Hand.Instance.GetCardData(handIndex).cost;
                //card is in monster box
                //FOR NOW PLACES CARD TO LEFT!!
                GameManager.Instance.PlayerPlayCard();
                WebSocketService.PlayCard(handIndex, yourMonsterZone.ghostCard.GetComponent<InGameCard>().indexOnField);

                //FOR NOW PLACES CARD TO LEFT!!
                yourMonsterZone.AddNewMonsterCard(true, 0, heldCard.GetComponent<InGameCard>().cardData);
                Hand.Instance.RemoveCard(handIndex);
            }
            else
            {
                yourMonsterZone.RemoveGhostCard();
                uiHand.ReturnVisibleCard(heldCard, handIndex);
                heldCard = null;
            }
        }
        else if(RayCaster.Instance.target == GameObject.Find("Bonfire"))
        {
            yourMonsterZone.RemoveGhostCard();
            Debug.Log("Card discarded from slot " + handIndex);
            WebSocketService.Burn(handIndex);
            GameManager.Instance.PlayerBurnCard(heldCard);
            Hand.Instance.RemoveCardNoDestroy(handIndex);
            heldCard = null;
        }
        else
        {
            yourMonsterZone.RemoveGhostCard();
            uiHand.ReturnVisibleCard(heldCard, handIndex);
            heldCard = null;
        }
    }
}
