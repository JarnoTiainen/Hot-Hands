using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardBurn : MonoBehaviour
{
    [SerializeField] private Material cardOutlineGlowMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    bool endingBurning;
    [SerializeField] private float burnStartDuration;
    [SerializeField] private float burnEndDuration;
    float time = 0;
    [SerializeField] private float speed;
    [SerializeField] private bool effectOn;
    [SerializeField] private bool reverseEffectOn;

    [SerializeField] [ColorUsage(true, true)] private Color canAffordColor;
    [SerializeField] [ColorUsage(true, true)] private Color attackReadyColor;

    private void Awake()
    {
        meshRenderer.material = cardOutlineGlowMaterial;
    }

    private void Update()
    {
        if(effectOn)
        {
            if (reverseEffectOn) time -= Time.deltaTime * speed;
            else time += Time.deltaTime * speed;
            if (time <= 1 && time >= 0)
            {
                meshRenderer.material.SetFloat("_AnimationStep", time);
            }
            else
            {
                effectOn = false;
                reverseEffectOn = false;
            }
        }
    }

    [Button]public void StartBurning()
    {
        time = 0;
        meshRenderer.material.SetColor("_Color", attackReadyColor);
        effectOn = true;
        
    }

    [Button]
    public void EndBurning()
    {
        time = 1;
        effectOn = true;
        reverseEffectOn = true;
    }

    [Button]public void StartCanAfford()
    {
        time = 0;
        meshRenderer.material.SetColor("_Color", canAffordColor);
        effectOn = true;
    }
    [Button]public void EndCanAfford()
    {
        time = 1;
        effectOn = true;
        reverseEffectOn = true;
    }

}
