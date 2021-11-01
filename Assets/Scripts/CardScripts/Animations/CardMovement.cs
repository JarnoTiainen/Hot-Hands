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
    private Vector3 startAttackPoint;
    public Vector3 endPoint;
    private Vector3 endAttackPoint;


    private float elapsedRotationTime;
    private float elapsedTime;
    private float elapsedAttackTime;

    private int dirMultiplier = 1;

    public bool doMove = false;
    private bool doRotate = false;
    public bool doAttack = false;

    [SerializeField] private float maxMovementRotateAngle;
    private Vector3 previousPos;


    [SerializeField] private GameObject targetCard;


    void Start()
    {  
        //this is so that we can set the used curve back to the default, if the curve value has been changed
        defaultCurve = curve;
    }

    void Update()
    {
        //priotize attacking over normal movement
        if (!doAttack) {
            if (doMove && transform.localPosition != endPoint) {
                elapsedTime += Time.deltaTime;
                //smoothly moves towards the endpoint. should this be in the fixedUpdate?
                transform.localPosition = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(elapsedTime / duration));
            } else if (doMove && transform.localPosition == endPoint) {  //if the card has moved to the destination, reset variables
                doMove = false;
                curve = defaultCurve;
            }
        }
        

        //using euler angles here because quaternions would be different, but the euler angles are same
        if(doRotate && transform.rotation.eulerAngles != endRotation.eulerAngles) {
            elapsedRotationTime += Time.deltaTime;
            
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, curve.Evaluate(elapsedRotationTime / rotationSpeed));
        } else if (doRotate && transform.rotation.eulerAngles == endRotation.eulerAngles) {
            doRotate = false;
        }


        if (doAttack && transform.position != endAttackPoint) {
            elapsedAttackTime += Time.deltaTime;
            //the amount of curve added to the path in x axis
            Vector3 addToX = new Vector3(attackCurve.Evaluate(elapsedAttackTime / attackDur) * curveMultiplier * dirMultiplier, 0, 0);

            transform.position = Vector3.Lerp(startAttackPoint, endAttackPoint, curve.Evaluate(elapsedAttackTime / attackDur));
            transform.position = new Vector3(transform.position.x * 1 + addToX.x, transform.position.y, transform.position.z);

        } else if (doAttack && transform.position == endAttackPoint) {  //if the card has moved to the destination, reset variables
            AttackEventHandler ah = References.i.attackEventHandler;
            int owner = GetComponent<InGameCard>().owner;
            ah.StartDamageEvent(owner, gameObject, targetCard);

            //if (!(targetCard == References.i.yourPlayerTarget || targetCard == References.i.enemyPlayerTarget)) {
            //    targetCard.GetComponent<CardMovement>().OnCardMove(targetCard.transform.position + new Vector3(0, 0, -.1f), 0.1f);
            //}

            doAttack = false;
            //if another script is trying to move this, let it do so
            if (doMove) {
                startPoint = transform.position;
            } else { 
                if(GameManager.Instance.IsYou(GetComponent<InGameCard>().owner)) OnCardMove(References.i.yourMonsterZone.transform.InverseTransformPoint(startAttackPoint), 0.6f);
                else OnCardMove(References.i.opponentMonsterZone.transform.InverseTransformPoint(startAttackPoint), 0.6f);
            } 
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

    public void OnCardAttack(GameObject target, float dur)
    {
        targetCard = target;
        startAttackPoint = transform.position;
        endAttackPoint = target.transform.position;
        attackDur = dur;
        elapsedAttackTime = 0;

        //if the card isn't attacking directly at you
        if (!(targetCard == References.i.yourPlayerTarget || targetCard == References.i.enemyPlayerTarget))
        {
            int multiplier;
            attackDur += 0.1f;
            //if the attacking card is the opponents, reverse the multiplier

            if (GameManager.Instance.IsYou(GetComponent<InGameCard>().owner)) {
                //the card is yours
                multiplier = 1;
                dirMultiplier = 1;
            } else {
                //if the card is not yours
                multiplier = -1;
                dirMultiplier = -1;
            }

            //set the attack point offset and direction for the attackcurve
            if (GetComponent<InGameCard>().cardData.attackDirection == Card.AttackDirection.Left) {
                endAttackPoint.x -= References.i.fieldCard.GetComponent<BoxCollider>().size.x * multiplier;
                dirMultiplier *= -1;
            } else if (GetComponent<InGameCard>().cardData.attackDirection == Card.AttackDirection.Right) {
                endAttackPoint.x += References.i.fieldCard.GetComponent<BoxCollider>().size.x * multiplier;
            }

        } else {
           //direct attack
            dirMultiplier = 0;
        }

        doAttack = true;
    }

    private IEnumerator TargetLift()
    {
        //move the target card up
        targetCard.GetComponent<CardMovement>().OnCardMove(targetCard.transform.position + new Vector3(0, 0, -.1f), 0.1f);
        yield return 0;
    }


}
