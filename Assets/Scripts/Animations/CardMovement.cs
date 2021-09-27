using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public GameObject discardGO;
    public Transform cardTrans;
    public Transform deckTrans;
    public Transform discardTrans;
    public float duration = 2;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float elapsedTime;
    private bool doMove = false;

    //specifies the movement of the card
    [SerializeField]
    private AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        cardTrans = GameObject.FindGameObjectWithTag("Card").transform;
        deckTrans = GameObject.FindGameObjectWithTag("Deck").transform;
        discardTrans = GameObject.FindGameObjectWithTag("DiscardPile").transform;
        discardGO = GameObject.FindGameObjectWithTag("DiscardPile");


        
        startPoint = cardTrans.position;
        //getting the second transform in this list because otherwise it would return original transform 
        endPoint = discardGO.GetComponentsInChildren<Transform>()[1].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            doMove = true;
        }

        if(doMove && cardTrans.position != endPoint) {
            elapsedTime += Time.deltaTime;
            //smoothly goes towards the endpoint. should this be in the fixedUpdate?
            cardTrans.position = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(elapsedTime / duration));
        }
    }
}
