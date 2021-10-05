using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private Material material;

    private float dissolveAmount = 1;
    [SerializeField] private bool isDissolving;


    private void Update()
    {
        if(isDissolving)
        {
            Debug.Log("loop");

            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            material.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }

}
