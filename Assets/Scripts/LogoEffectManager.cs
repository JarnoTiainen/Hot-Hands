using System.Collections;
using UnityEngine;

public class LogoEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float animationDuration = 3;

    void Start()
    {
        meshRenderer.material = material;
        StartCoroutine(Animation());
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
}