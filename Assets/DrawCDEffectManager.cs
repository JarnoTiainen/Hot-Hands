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
        deckCardCount.text = GameManager.Instance.playerStats.deckCardCount.ToString();
        if (animating)
        {
            time += Time.deltaTime/effDur;
            image.fillAmount = time;
            if(time > 1)
            {
                animating = false;
                image.fillAmount = 0;
            }
        }
    }

    [Button] public void StartDrawCDEffect(float duration = 3)
    {
        time = 0;
        animating = true;
        effDur = duration;
    }
}
