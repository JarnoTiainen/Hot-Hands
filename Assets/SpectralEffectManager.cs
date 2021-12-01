using UnityEngine;
using Sirenix.OdinInspector;

public class SpectralEffectManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    [SerializeField] bool turningOn;
    [SerializeField] bool turningOff;

    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        if(turningOn)
        {
            time += Time.deltaTime;
            if (time > 0.5f) turningOn = false;
        }
        else if(turningOff)
        {
            time -= Time.deltaTime;
            if (time < 0) turningOff = false;
        }
        meshRenderer.material.SetFloat("_AnimationStep", time);
    }

    [Button] public void StartEffect()
    {
        turningOn = true;
        turningOff = false;
        time = 0;
    }

    [Button] public void StopEffect()
    {
        turningOff = true;
        turningOn = false;
        time = 0.5f;
    }
}
