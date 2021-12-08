using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



public class HighLightController : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] MeshRenderer meshRenderer;

    float speed = 1;
    bool animating = false;
    bool animatingBack = false;
    float time = 0;
    public bool effectOn = false;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.material = material;
    }

    public void Update()
    {
        if(animating)
        {
            time += speed * Time.unscaledDeltaTime;
            meshRenderer.material.SetFloat("_FadeInStep", time);

            if(time >= 1)
            {
                animating = false;
            }
        }
        else if(animatingBack)
        {
            time -= speed * Time.unscaledDeltaTime;
            meshRenderer.material.SetFloat("_FadeInStep", time);

            if (time < 0)
            {
                animatingBack = false;
            }
        }
    }

    [Button]public void ToggleHighlightAnimation()
    {

        if(effectOn)
        {
            effectOn = false;
            animatingBack = true;
            time = 1;
        }
        else
        {
            effectOn = true;
            animating = true;
            time = 0;
        }
    }

}
