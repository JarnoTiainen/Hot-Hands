using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public TMPro.TextMeshProUGUI burnValue;

    [SerializeField] private BonfireEffectManager bonfireEffectManager;


    public void PlayEffect()
    {
        bonfireEffectManager.PlayEffect();
    }
}
