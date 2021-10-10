using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardBurn : MonoBehaviour
{
    [SerializeField] private Shader burnShader;
    [SerializeField] private MeshRenderer meshRendererBurnTop;
    [SerializeField] private MeshRenderer meshRendererBurnRight;
    [SerializeField] private MeshRenderer meshRendererBurnLeft;
    [SerializeField] private MeshRenderer meshRendererBurnBottom;
    private Material mat1;
    private Material mat2;
    private Material mat3;
    private Material mat4;

    bool startingBurning;
    bool endingBurning;
    [SerializeField] private float burnStartDuration;
    [SerializeField] private float burnEndDuration;
    float timer = 0;

    private void Awake()
    {
        mat1 = meshRendererBurnTop.material;
        mat2 = meshRendererBurnRight.material;
        mat3 = meshRendererBurnLeft.material;
        mat4 = meshRendererBurnBottom.material;
        mat1.SetVector("_TextureOffset", new Vector2(Random.Range(0,3), Random.Range(0,3)));
        mat2.SetVector("_TextureOffset", new Vector2(Random.Range(0, 3), Random.Range(0, 3)));
        mat3.SetVector("_TextureOffset", new Vector2(Random.Range(0, 3), Random.Range(0, 3)));
        mat4.SetVector("_TextureOffset", new Vector2(Random.Range(0, 3), Random.Range(0, 3)));
        meshRendererBurnTop.material.shader = burnShader;
        meshRendererBurnRight.material.shader = burnShader;
        meshRendererBurnLeft.material.shader = burnShader;
        meshRendererBurnBottom.material.shader = burnShader;
    }

    private void Update()
    {
        if (timer > 0 && startingBurning)
        {
            timer -= Time.deltaTime;
            mat1.SetFloat("_Transparency", 1 - 1 * timer / burnStartDuration);
            mat2.SetFloat("_Transparency", 1 - 1 * timer / burnStartDuration);
            mat3.SetFloat("_Transparency", 1 - 1 * timer / burnStartDuration);
            mat4.SetFloat("_Transparency", 1 - 1 * timer / burnStartDuration);
            if (timer <= 0)
            {
                startingBurning = false;
            }
        }

        if (timer > 0 && endingBurning)
        {
            timer -= Time.deltaTime;
            mat1.SetFloat("_Transparency", 1 * timer / burnEndDuration);
            mat2.SetFloat("_Transparency", 1 * timer / burnEndDuration);
            mat3.SetFloat("_Transparency", 1 * timer / burnEndDuration);
            mat4.SetFloat("_Transparency", 1 * timer / burnEndDuration);
            if (timer <= 0)
            {
                endingBurning = false;
            }
        }
    }

    [Button]public void StartBurning()
    {
        mat1.SetInt("_BurnOrAfford", 1);
        mat2.SetInt("_BurnOrAfford", 1);
        mat3.SetInt("_BurnOrAfford", 1);
        mat4.SetInt("_BurnOrAfford", 1);
        timer = burnStartDuration;
        startingBurning = true;
    }

    [Button]
    public void EndBurning()
    {
        
        timer = burnEndDuration;
        endingBurning = true;
    }

    [Button]public void StartCanAfford()
    {
        mat1.SetInt("_BurnOrAfford", 0);
        mat2.SetInt("_BurnOrAfford", 0);
        mat3.SetInt("_BurnOrAfford", 0);
        mat4.SetInt("_BurnOrAfford", 0);
        timer = burnStartDuration;
        startingBurning = true;
    }
    [Button]public void EndCanAfford()
    {
        timer = burnEndDuration;
        endingBurning = true;
    }

}
