using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HighLightController : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.material = material;
    }

    [Button]public void ToggleHighlightAnimation()
    {
        if(meshRenderer.material.GetInt("_HighlightOn") == 1)
        {
            meshRenderer.material.SetInt("_HighlightOn", 0);
        }
        else
        {
            meshRenderer.material.SetInt("_HighlightOn", 1);
        }
    }

}
