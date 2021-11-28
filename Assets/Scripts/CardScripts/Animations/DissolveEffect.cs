using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    private Material material;

    private float dissolveAmount = 0f;
    [SerializeField] private bool isDissolving;
    [SerializeField] private bool isReverseDissolving;
    private float time = 0;
    [SerializeField] private float dissolveSpeed = 1;
    [SerializeField] private float dissolveSpeedReverse = 1;

    private void Update()
    {
        if(isDissolving)
        {
            time += Time.deltaTime * dissolveSpeed;
            if(time > 1)
            {
                isDissolving = false;
            }
            material.SetFloat("_DissolveAmount", time);
        }
        if(isReverseDissolving)
        {
            time -= Time.deltaTime * dissolveSpeedReverse;
            if (time < 0)
            {
                isReverseDissolving = false;
                transform.GetChild(1).gameObject.SetActive(true);
            }
            material.SetFloat("_DissolveAmount", time);
        }
    }

    public void StartDissolving(Material material)
    {
        isReverseDissolving = true;
        time = 0;
        this.material = material;
        isDissolving = true;
    }
    public void StartReverseDissolving(Material material)
    {
        isDissolving = false;
        time = 1;
        this.material = material;
        isReverseDissolving = true;
        transform.GetChild(1).gameObject.SetActive(false);
        material.SetFloat("_DissolveAmount", 1);
    }
}
