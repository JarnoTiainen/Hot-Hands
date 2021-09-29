using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mouse : MonoBehaviour
{
    private Canvas UiCanvas;
    private GameObject heldCard;
    public Camera thisCamera;
    [SerializeField] private Vector2 monsterHitBox;
    public Vector2 mousePosInWorld;
    public GameObject markerPrefab;
    private UiHand uiHand;
    [SerializeField] private bool debuggingOn = false;

    private void Awake()
    {
        UiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
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

    [Button] public void SetNewHeldCard(GameObject newCard)
    {
        heldCard = newCard;
        newCard.transform.SetParent(gameObject.transform);
        newCard.transform.localPosition = Vector3.zero;
    }

    public void ValuatePlaceCard()
    {
        Debug.Log("valuate");
        if (Mathf.Abs(mousePosInWorld.x) < monsterHitBox.x && Mathf.Abs(mousePosInWorld.y) < monsterHitBox.y)
        {
            //card is in monster box
            Debug.Log("place card");
        }
        else
        {
            Debug.Log("Return card");
            uiHand.ReturnVisibleCard(heldCard);
            heldCard = null;
        }
    }
}
