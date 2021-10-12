using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public float duration = 0.5f;
    public float attackDur;
    public float rotationSpeed = 0.5f;
    public float curveMultiplier = 0.1f;

    //specifies the movement curve of the card
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private AnimationCurve attackCurve;
    private AnimationCurve defaultCurve;
    private AnimationCurve smoothAngleTransition;

    private Quaternion startRotation;
    private Quaternion endRotation;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private float elapsedRotationTime;
    private float elapsedTime;
    private float elapsedAttackTime;

    private bool doMove = false;
    private bool doRotate = false;
    private bool doAttack = false;

    [SerializeField] private float maxMovementRotateAngle;
    private Vector3 previousPos;




    void Start()
    {  
        //this is so that we can set the used curve back to the default, if the curve value has been changed
        defaultCurve = curve;
    }

    void Update()
    {
        //Vector3 normalizedDirection = (transform.position - previousPos).normalized;
        //Vector3 goalRotation = normalizedDirection * maxMovementRotateAngle;


        if (doMove && transform.localPosition != endPoint) {
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

        if (doAttack && transform.localPosition != endPoint) {
            elapsedAttackTime += Time.deltaTime;

            Vector3 addToX = new Vector3(attackCurve.Evaluate(elapsedAttackTime / attackDur) * curveMultiplier, 0, 0);
            //Debug.Log("Added x " + addToX.x + " curve " + attackCurve.Evaluate(elapsedAttackTime / attackDur) + " multiplier " + curveMultiplier + " elapsed time" + elapsedAttackTime + " duration " + attackDur);


            transform.localPosition = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(elapsedAttackTime / attackDur));
            transform.localPosition = new Vector3(transform.localPosition.x * 1 + addToX.x, transform.localPosition.y, transform.localPosition.z);

        } else if (doAttack && transform.localPosition == endPoint) {  //if the card has moved to the destination, reset variables
            doAttack = false;
            OnCardMove(startPoint, 0.6f);
        }
    }
    ///uses the default animation curve of card, startpoint specifiable
    public void OnCardMove(Vector3 startP, Vector3 endP, float dur)
    {
        startPoint = startP;
        endPoint = endP;
        duration = dur;
        elapsedTime = 0;
        doMove = true;
    }

    ///uses a specified animation curve
    public void OnCardMove(Vector3 endP, float dur, AnimationCurve curv)
    {
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        curve = curv;
        elapsedTime = 0;
        doMove = true;
    }

    ///uses the default animation curve of card
    public void OnCardMove(Vector3 endP, float dur)
    {
        startPoint = transform.localPosition;
        endPoint = endP;
        duration = dur;
        elapsedTime = 0;
        doMove = true;
    }

    ///rotates card the amount of "rotation" parameter
    public void OnCardRotate(Quaternion rotation, float rotSpeed)
    {
        startRotation = transform.rotation;
        endRotation = rotation;
        rotationSpeed = rotSpeed;
        elapsedRotationTime = 0;
        doRotate = true;
    }

    public void OnCardAttack(Vector3 endP, float dur)
    {
        startPoint = transform.localPosition;
        endPoint = endP;
        attackDur = dur;
        elapsedAttackTime = 0;
        doAttack = true;
    }

}
