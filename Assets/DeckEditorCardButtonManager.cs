using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeckEditorCardButtonManager : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Button buttonType;
    private BuildCardScript buildCardScript;
    private enum Button
    {
        Add,
        Remove
    }

    private void Awake()
    {
        meshRenderer.material = material;
        buildCardScript = gameObject.transform.parent.gameObject.GetComponent<BuildCardScript>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        meshRenderer.material.SetFloat("_AnimationStep", 0);
    }

    [Button]
    public IEnumerator Animation()
    {
        float progress = 1;
        while(progress > 0)
        {
            progress -= (Time.deltaTime / animationDuration);
            meshRenderer.material.SetFloat("_AnimationStep", progress);
            yield return null;
        }
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
        if (buttonType == Button.Add) buildCardScript.AddCard();
        else if (buttonType == Button.Remove) buildCardScript.DeleteCard();
        if(gameObject.activeSelf) StartCoroutine(Animation());
    }
}
