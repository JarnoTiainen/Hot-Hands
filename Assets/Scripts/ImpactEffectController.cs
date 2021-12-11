using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectController : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float speed;
    private float time;


    private void Awake()
    {
        meshRenderer.material = material;
    }

    private void Update()
    {
        time += Time.deltaTime * speed;
        meshRenderer.material.SetFloat("_AnimationStep", time);
        if (time > 1) Destroy(gameObject);
    }
}
