using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CollectionArrowManager : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float speed;
    private float time;
    private bool animating;
    [SerializeField] private int buttonValue;
    public void Update()
    {
        if(animating)
        {
            time -= speed * Time.deltaTime;
            if(time < 0)
            {
                animating = false;
            }
            meshRenderer.material.SetFloat("_AnimationStep", time);
        }
    }

    [Button]public void StartAnimation()
    {
        time = 1;
        animating = true;
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.SetInt("_IsHovered", 1);
    }

    public void OnHoverExit()
    {
        meshRenderer.material.SetInt("_IsHovered", 0);
    }

    public void OnClickElement()
    {
        StartAnimation();
        CollectionManager.Instance.ChangePage(buttonValue);
    }
}
