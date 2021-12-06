using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameOverEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    float time = 0;
    private bool animating;
    [SerializeField] private float speed;

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
            time += Time.deltaTime * speed;
            if(time > 1)
            {
                animating = false;
            }
            meshRenderer.material.SetFloat("_AnimationStep", time);
        }
    }

    [Button] public void StartAnimation()
    {
        animating = true;
        time = 0;
    }


}
