using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardRuneEffectManager : MonoBehaviour
{
    [SerializeField] private Material runeEffectMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    private float time = 0;
    [SerializeField] private float speed;

    [SerializeField] private GameObject puffEffectPrefab;
    [SerializeField] private float puffEffectTime;
    [SerializeField] private InGameCard card;
    private bool puffDone;
    [SerializeField] private bool effectOn;
    [SerializeField] private bool reverseOn;


    private void Awake()
    {
        meshRenderer.material = runeEffectMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(effectOn)
        {
            time += Time.deltaTime * speed;

            meshRenderer.material.SetFloat("_AnimationStep", time);
            if (time > puffEffectTime && !puffDone)
            {
                Instantiate(puffEffectPrefab, transform.position, Quaternion.identity);
                card.ToggleGhostCard(true);
                puffDone = true;
            }

            if(time > 1)
            {
                time = 1;
                effectOn = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else if(reverseOn)
        {
            time -= Time.deltaTime * speed;
            meshRenderer.material.SetFloat("_AnimationStep", time);
            if (time < puffEffectTime && !puffDone)
            {
                card.ToggleGhostCard(false);
                puffDone = true;
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
        transform.GetChild(0).gameObject.SetActive(true);
        puffDone = false;
        effectOn = true;
        time = 0;
    }

    [Button]
    public void PlayReverseRuneEffect()
    {
        puffDone = false;
        effectOn = false;
        reverseOn = true;
    }
}
