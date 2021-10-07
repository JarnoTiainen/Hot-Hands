using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissolveMover : MonoBehaviour
{
    public AnimationCurve scaleCurve;
    public AnimationCurve spinCurve;
    public AnimationCurve movementCurve;
    public float velocityM;
    public float rotationCount;
    [SerializeField] private float lerpTime;
    private float timer = 0f;
    [SerializeField] private bool isDissolving = false;
    private float startScale;
    Vector3 startPos;

    void Update()
    {
        if (isDissolving)
        {
            if (timer < lerpTime) timer += Time.deltaTime;
            if (timer > lerpTime) timer = lerpTime;
            float lerpRatio = timer / lerpTime;

            float scale = scaleCurve.Evaluate(lerpRatio);
            float rotationScale = spinCurve.Evaluate(lerpRatio);

            GetComponent<Transform>().localPosition = Vector3.Lerp(startPos, Vector3.zero, lerpRatio);
            GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, rotationScale * rotationCount * 360);
            GetComponent<Transform>().localScale = new Vector3(scale, scale, scale) * startScale;
        }
            
    }

    public void StartDissolving()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale.x;
        isDissolving = true;
    }
}
