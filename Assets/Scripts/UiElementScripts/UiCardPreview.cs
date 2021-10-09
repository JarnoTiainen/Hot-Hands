using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiCardPreview : MonoBehaviour
{
    public AnimationCurve curve;
    [SerializeField] private Vector3 lerpOffset;
    [SerializeField] private float lerpTime;
    private float timer = 0f;
    public Vector3 targetPos;
    public Vector3 startPos;


    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;

    void Update()
    {
        if(timer < lerpTime) timer += Time.deltaTime;
        if (timer > lerpTime) timer = lerpTime;
        float lerpRatio = timer / lerpTime;

        Vector3 positionOffset = curve.Evaluate(lerpRatio) * lerpOffset;

        GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPos, targetPos, lerpRatio) + positionOffset;
    }

    public void SetNewCardData(CardData cardData)
    {
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();
        if (value != null) lp.text = cardData.lp.ToString();
        if (value != null) rp.text = cardData.rp.ToString();
    }
}
