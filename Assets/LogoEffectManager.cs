using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    [SerializeField] private float time;
    [SerializeField] private bool animating = true;
    [SerializeField] private float animationTime = 3;



    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        if(animating)
        {
            time += Time.deltaTime / animationTime;
            meshRenderer.material.SetFloat("_AnimationStep", time);
        }
    }
}
