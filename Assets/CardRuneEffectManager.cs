using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardRuneEffectManager : MonoBehaviour
{
    [SerializeField] private Material runeEffectMaterial;
    [SerializeField] private Material destroyEffectMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    private float time = 0;
    [SerializeField] private float runeEffectspeed;
    [SerializeField] private float destroyEffectspeed;

    [SerializeField] private GameObject puffEffectPrefab;
    [SerializeField] private GameObject triangleShatterEffectPrefab;
    [SerializeField] private float runeEffectPuffEffectTime;
    [SerializeField] private float destroyEffectPuffEffectTime;
    [SerializeField] private InGameCard card;
    private bool puffDone;
    [SerializeField] private bool effectOn;
    [SerializeField] private bool reverseOn;

    [SerializeField] private EffectType effectType;

    private enum EffectType
    {
        RuneBurn, Destroy
    }

    private void Awake()
    {
        meshRenderer.material = runeEffectMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(effectOn)
        {
            if (effectType == EffectType.RuneBurn) time += Time.deltaTime * runeEffectspeed;
            else if (effectType == EffectType.Destroy) time += Time.deltaTime * destroyEffectspeed;


            meshRenderer.material.SetFloat("_AnimationStep", time);
            if (effectType == EffectType.RuneBurn)
            {
                if (time > runeEffectPuffEffectTime && !puffDone)
                {
                    Debug.Log("rune");
                    Instantiate(puffEffectPrefab, transform.position, Quaternion.identity);
                    card.ToggleGhostCard(true);
                    puffDone = true;
                }
            }
            else if (effectType == EffectType.Destroy)
            {
                if (time > destroyEffectPuffEffectTime && !puffDone)
                {
                    Debug.Log("destroy");
                    Instantiate(triangleShatterEffectPrefab, transform.position, Quaternion.identity);
                    card.ToggleGhostCard(true);
                    puffDone = true;
                }
            }


            if (time > 1)
            {
                time = 1;
                effectOn = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else if(reverseOn)
        {
            Debug.Log(time);

            if (effectType == EffectType.RuneBurn) time -= Time.deltaTime * runeEffectspeed;
            else if (effectType == EffectType.Destroy) time -= Time.deltaTime * destroyEffectspeed;

            meshRenderer.material.SetFloat("_AnimationStep", time);


            if (effectType == EffectType.RuneBurn)
            {
                if (time < runeEffectPuffEffectTime && !puffDone)
                {
                    card.ToggleGhostCard(false);
                    puffDone = true;
                }
            }
            else if (effectType == EffectType.Destroy)
            {
                if (time < destroyEffectPuffEffectTime && !puffDone)
                {
                    card.ToggleGhostCard(false);
                    puffDone = true;
                }
            }

            

            if (time < 0)
            {
                time = 0;
                reverseOn = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    [Button]public void PlayRuneEffect()
    {
        effectType = EffectType.RuneBurn;
        meshRenderer.material = runeEffectMaterial;
        transform.GetChild(0).gameObject.SetActive(true);
        puffDone = false;
        effectOn = true;
        time = 0;
    }

    [Button]
    public void PlayReverseRuneEffect()
    {
        effectType = EffectType.RuneBurn;
        meshRenderer.material = runeEffectMaterial;
        transform.GetChild(0).gameObject.SetActive(true);
        puffDone = false;
        effectOn = false;
        reverseOn = true;
    }

    [Button]
    public void PlayRuneDestroyEffect()
    {
        effectType = EffectType.Destroy;
        meshRenderer.material = destroyEffectMaterial;
        transform.GetChild(0).gameObject.SetActive(true);
        puffDone = false;
        effectOn = true;
        time = 0;
    }

    [Button]
    public void PlayReverseDestroyEffect()
    {
        effectType = EffectType.Destroy;
        meshRenderer.material = destroyEffectMaterial;
        transform.GetChild(0).gameObject.SetActive(true);
        puffDone = false;
        effectOn = false;
        reverseOn = true;
    }
}
