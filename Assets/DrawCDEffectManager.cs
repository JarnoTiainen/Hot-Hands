using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DrawCDEffectManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TextMeshProUGUI deckCardCount;

    [SerializeField] private float time;
    [SerializeField] private bool animating;
    private float effDur = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animating)
        {
            time += Time.deltaTime/effDur;
            image.fillAmount = time;
            if(time > 0)
            {
                Debug.Log("ASread<gsty gfdszgfdzas");
                animating = false;
                image.fillAmount = 0;
            }
        }
    }

    [Button] public void StartDrawCDEffect(float duration)
    {
        time = 0;
        animating = true;
        effDur = duration;
    }
}
