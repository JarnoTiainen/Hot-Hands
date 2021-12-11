using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardTakeDamageManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    private float time = 0;
    [SerializeField] private float speed = 1;
    private bool effectOn;

    private void Awake()
    {
        meshRenderer.material = material;
    }

    void Update()
    {
        if(effectOn)
        {
            time += Time.deltaTime * speed;
            meshRenderer.material.SetFloat("_AnimationStep", time);
            if(time > 1)
            {
                time = 0;
                effectOn = false;
            }
        }
    }

    [Button] public void PlayEffect()
    {
        time = 0;
        effectOn = true;
    }
}
