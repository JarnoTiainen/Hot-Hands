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
    private UiHand uiHand;
    [SerializeField] private bool debuggingOn = false;

    private void Awake()
    {
        Instance = gameObject.GetComponent<Mouse>();
        uiHand = GameObject.Find("Hand").GetComponent<UiHand>();
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
            transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, 0);
        }
        if(Input.GetMouseButtonUp(0) && heldCard)
        {
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

    public void ValuatePlaceCard()
    {
        if (Mathf.Abs(mousePosInWorld.x) < monsterHitBox.x && Mathf.Abs(mousePosInWorld.y) < monsterHitBox.y)
        {
            //card is in monster box
            Debug.Log("place card");
            yourMonsterZone.AddNewMonsterCard();
            Debug.Log("Removing card with i " + handIndex);
            UiHand.Instance.RemoveCard(handIndex);
        }
        else
        {
            uiHand.ReturnVisibleCard(heldCard);
            heldCard = null;
        }
    }
}
