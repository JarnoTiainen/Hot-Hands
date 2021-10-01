using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public float duration = 2;

    //specifies the movement curve of the card
    [SerializeField]
    private AnimationCurve curve;
    private AnimationCurve defaultCurve;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float elapsedTime;
    private bool doMove = false;


    void Start()
    {  
        //this is so that we can set the used curve back to the default, if the curve value has been changed
        defaultCurve = curve;
        
        startPoint = transform.position;
        endPoint = transform.position;
    }

    void Update()
    {
        if(doMove && transform.localPosition != endPoint) {
            Debug.Log("In move yea");
            elapsedTime += Time.deltaTime;
            //smoothly moves towards the endpoint. should this be in the fixedUpdate?

            transform.localPosition = endPoint;
            //transform.localPosition = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(elapsedTime / duration));
        } else if (doMove && transform.localPosition == endPoint) {  //if the card has moved to the destination, reset variables
            doMove = false;
            curve = defaultCurve;
            elapsedTime = 0;
        }
    }

    //uses a specified animation curve
    public void OnCardMove(Vector2 endP, float dur, AnimationCurve curv)
    {
        Debug.Log("onCardmove default startpoint and specified curve");
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        curve = curv;
        doMove = true;
    }

    //uses the default animation curve of card
    public void OnCardMove(Vector2 endP, float dur)
    {
        Debug.Log("onCardmove default startpoint and curve");
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        doMove = true;
    }

    //uses the default animation curve of card, startpoint specifiable
    public void OnCardMove(Vector2 startP, Vector2 endP, float dur)
    {
        Debug.Log("onCardmove specified startpoint and curve");
        startPoint = startP;
        endPoint = endP;
        duration = dur;
        doMove = true;
    }
}
