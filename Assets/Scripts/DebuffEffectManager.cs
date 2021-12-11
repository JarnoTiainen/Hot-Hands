using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DebuffEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float time = 1;
    [SerializeField] private float speed = 1;
    private bool effectOn;

    private void Awake()
    {
        meshRenderer.material = material;
    }

    void Update()
    {
        if (effectOn)
        {
            time += Time.deltaTime * speed;
            meshRenderer.material.SetFloat("_AnimationStep", time);
            if (time > 1)
            {
                time = 1;
                effectOn = false;
            }
        }
    }

    [Button]
    public void PlayEffect()
    {
        time = 0;
        effectOn = true;
    }
}
