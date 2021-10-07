using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveShadow : MonoBehaviour
{
    [SerializeField] private GameObject shadowCaster;
    [SerializeField] private float dissolveSpeed;
    private float dissolveAmount = 1;
    [SerializeField] private bool isDissolving;

    private void Update()
    {
        if (isDissolving)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime * dissolveSpeed);
            shadowCaster.transform.localScale = new Vector3(dissolveAmount, dissolveAmount, dissolveAmount);
        }
    }
    public void StartDissolving()
    {
        isDissolving = true;
    }
}
