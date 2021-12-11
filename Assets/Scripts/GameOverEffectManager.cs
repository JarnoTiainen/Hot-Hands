using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameOverEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float animationDuration = 1f;

    void Start()
    {
        meshRenderer.material = material;
    }

    private IEnumerator Animation()
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += (Time.deltaTime / animationDuration);
            meshRenderer.material.SetFloat("_AnimationStep", progress);
            yield return null;
        }
    }

    [Button] public void StartAnimation()
    {
        StartCoroutine(Animation());
    }
}
