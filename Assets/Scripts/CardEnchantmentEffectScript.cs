using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardEnchantmentEffectScript : MonoBehaviour
{
    public GameObject lastBreathEffect;
    public GameObject openerEffect;
    public GameObject sacrificeEffect;
    public GameObject wildEffect;
    public float maxEffectVisibilityTime;
    public float currentTimer = 0;

    [Button] public void PlayEffectLastBreath()
    {
        currentTimer = maxEffectVisibilityTime;
        lastBreathEffect.SetActive(true);
    }
    [Button]
    public void PlayEffectOpener()
    {
        currentTimer = maxEffectVisibilityTime;
        openerEffect.SetActive(true);
    }
    [Button]
    public void PlayEffectSacrifice()
    {
        currentTimer = maxEffectVisibilityTime;
        sacrificeEffect.SetActive(true);
    }
    [Button]
    public void PlayEffectWild()
    {
        currentTimer = maxEffectVisibilityTime;
        wildEffect.SetActive(true);
    }


    public void Update()
    {
        if(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            if(currentTimer <= 0)
            {
                StopEffects();
            }
        }
    }

    public void StopEffects()
    {
        lastBreathEffect.SetActive(false);
        openerEffect.SetActive(false);
        sacrificeEffect.SetActive(false);
        wildEffect.SetActive(false);
    }
}
