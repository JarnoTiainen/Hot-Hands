using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mouse : MonoBehaviour
{
    public static Mouse Instance { get; private set; }

    public GameObject heldCard;
    private int handIndex;
    [SerializeField] private Vector2 monsterHitBox;
    public Vector2 mousePosInWorld;
    public GameObject markerPrefab;
    [SerializeField] private bool debuggingModeOn = false;
    [SerializeField] private float mouseHightFromTableTop;
    [SerializeField] public bool targetModeOn;
    [SerializeField] public GameObject targetSource;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePosInWorld = raycastHit.point;
            transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, mouseHightFromTableTop);
        }
        if(Input.GetMouseButtonUp(0) && heldCard)
        {
            if(debuggingModeOn) Debug.Log("placing down");
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

            if (GameManager.Instance.playerStats.playerBurnValue >= Hand.Instance.GetCardData(handIndex).cost && GameManager.Instance.playerStats.playerFieldCards < GameManager.Instance.maxFieldCardCount)
            {
                GameManager.Instance.playerStats.playerFieldCards++;
                WebSocketService.PlayCard(handIndex, References.i.yourMonsterZone.monsterCards.IndexOf(References.i.yourMonsterZone.ghostCard));
                GameManager.Instance.PlayerPlayCard(heldCard.GetComponent<InGameCard>().cardData, handIndex);
                heldCard = null;
            }
            else
            {
                References.i.yourMonsterZone.RemoveGhostCard();
                Hand.Instance.ReturnVisibleCard(heldCard, handIndex);
                heldCard = null;
            }
        }
        else if(RayCaster.Instance.target == References.i.yourBonfire)
        {
            References.i.yourMonsterZone.RemoveGhostCard();
            if(debuggingModeOn) Debug.Log("Card discarded from slot " + handIndex);
            WebSocketService.Burn(handIndex);
            GameManager.Instance.PlayerBurnCard(heldCard);
            Hand.Instance.RemoveCardNoDestroy(handIndex);
            heldCard = null;
        }
        else
        {
            References.i.yourMonsterZone.RemoveGhostCard();
            Hand.Instance.ReturnVisibleCard(heldCard, handIndex);
            heldCard = null;
        }
    }
}
