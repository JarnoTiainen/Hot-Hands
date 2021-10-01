using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardPreview : MonoBehaviour
{
    public AnimationCurve curve;
    [SerializeField] private Vector3 lerpOffset;
    [SerializeField] private float lerpTime;
    private float timer = 0f;
    public Vector3 targetPos;
    public Vector3 startPos;

    void Update()
    {
        if(timer < lerpTime) timer += Time.deltaTime;
        if (timer > lerpTime) timer = lerpTime;
        float lerpRatio = timer / lerpTime;

        Vector3 positionOffset = curve.Evaluate(lerpRatio) * lerpOffset;

        GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPos, targetPos, lerpRatio) + positionOffset;
    }
}
