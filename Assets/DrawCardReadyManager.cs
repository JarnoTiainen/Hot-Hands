using UnityEngine;
using Sirenix.OdinInspector;

public class DrawCardReadyManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    private float time = 0;
    private float speed = 5;

    private bool animating;
    private bool reversing;


    private void Awake()
    {
        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        if(animating)
        {
            time += Time.deltaTime * speed;
            if (time > 1) animating = false;
        }
        if(reversing)
        {
            time -= Time.deltaTime * speed;
            if (time < 0) reversing = false;
        }
        meshRenderer.material.SetFloat("_AnimationStep", time);
    }

    [Button] public void StartAnimation()
    {
        time = 0;
        animating = true;
        reversing = false;
    }

    [Button] public void StopAnimation()
    {
        Debug.Log("stopping animation");
        time = 1;
        reversing = true;
        animating = false;
    }
}
