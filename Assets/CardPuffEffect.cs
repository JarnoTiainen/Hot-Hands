using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPuffEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystemRenderer effectParticleRenderer;
    [SerializeField] private Material cardMaterial;
    private float currentTranparency;
    [SerializeField] private float lifetime;
    [SerializeField] private float currentLifetime;


    private void Awake()
    {
        effectParticleRenderer.material = cardMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        currentLifetime -= Time.deltaTime;
        currentTranparency = currentLifetime / lifetime;
        Debug.Log(effectParticleRenderer.material.GetFloat("_TransparencyAmount") + " " + currentTranparency);
        effectParticleRenderer.material.SetFloat("_TransparencyAmount", currentTranparency);
        if (currentLifetime < 0) Destroy(gameObject);
    }
}
