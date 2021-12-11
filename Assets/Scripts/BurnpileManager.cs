using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BurnpileManager : MonoBehaviour
{
    [SerializeField] private int owener;
    [SerializeField] [Range(0, 20)] private float visibility;
    [SerializeField] private float zlow;
    [SerializeField] private GameObject burnpile;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    [SerializeField] private float time;
    [SerializeField] private float speed;
    private bool animationOn;

    private void Start()
    {
        meshRenderer.material = material;
    }

    void Update()
    {
        if(owener == 0)
        {
            visibility = GameManager.Instance.playerStats.discardpileCardCount;
        }
        else if (owener == 1)
        {
            visibility = GameManager.Instance.enemyPlayerStats.discardpileCardCount;
        }
        burnpile.transform.localPosition = new Vector3(0, 0, zlow * (1-visibility/20));

        if(animationOn)
        {
            time += Time.deltaTime * speed;
            meshRenderer.material.SetFloat("_AnimationStep", time);
            if (time > 1) {
                animationOn = false;
            }
        }
    }
        
    [Button] public void Animate()
    {
        time = 0;
        animationOn = true;
    }
}
