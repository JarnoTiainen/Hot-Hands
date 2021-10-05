using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public float duration = 0.5f;
    public float rotationSpeed = 0.5f;

    //specifies the movement curve of the card
    [SerializeField]
    private AnimationCurve curve;
    private AnimationCurve defaultCurve;

    private Quaternion startRotation;
    private Quaternion endRotation;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private float elapsedRotationTime;
    private float elapsedTime;

    private bool doMove = false;
    private bool doRotate = false;


    void Start()
    {  
        //this is so that we can set the used curve back to the default, if the curve value has been changed
        defaultCurve = curve;
    }

    void Update()
    {
        if(doMove && transform.localPosition != endPoint) {
            elapsedTime += Time.deltaTime;
            //smoothly moves towards the endpoint. should this be in the fixedUpdate?
            transform.localPosition = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(elapsedTime / duration));
        } else if (doMove && transform.localPosition == endPoint) {  //if the card has moved to the destination, reset variables
            doMove = false;
            curve = defaultCurve;
        }

        //using euler angles here because quaternions would be different, but the euler angles are same
        if(doRotate && transform.rotation.eulerAngles != endRotation.eulerAngles) {
            elapsedRotationTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, curve.Evaluate(elapsedRotationTime / rotationSpeed));
        } else if (doRotate && transform.rotation.eulerAngles == endRotation.eulerAngles) {
            doRotate = false;
        }
    }
    ///uses the default animation curve of card, startpoint specifiable
    public void OnCardMove(Vector3 startP, Vector3 endP, float dur)
    {
        startPoint = startP;
        endPoint = endP;
        duration = dur;
        doMove = true;
        elapsedTime = 0;
    }

    ///uses a specified animation curve
    public void OnCardMove(Vector3 endP, float dur, AnimationCurve curv)
    {
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        curve = curv;
        doMove = true;
        elapsedTime = 0;
    }

    ///uses the default animation curve of card
    public void OnCardMove(Vector3 endP, float dur)
    {
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        doMove = true;
        elapsedTime = 0;
    }

    ///rotates card the amount of "rotation" parameter
    public void OnCardRotate(Quaternion rotation, float rotSpeed)
    {
        startRotation = transform.rotation;
        endRotation = rotation;
        rotationSpeed = rotSpeed;
        doRotate = true;
        elapsedRotationTime = 0;
    }


}
