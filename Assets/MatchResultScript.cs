using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MatchResultScript : MonoBehaviour
{
    public static MatchResultScript Instance { get; private set; }
    [SerializeField] private EscMenu escMenuManager;
    public GameObject resultScreenButtons;
    [SerializeField] private GameObject resultQuitConfirmation;
    [SerializeField] private Image panelDim;
    [SerializeField] private GameObject victoryEffect;
    [SerializeField] private GameObject defeatEffect;
    [SerializeField] private int dimMax;
    [SerializeField] private int dimEnd;
    [SerializeField] private float dimMaxFade;
    [SerializeField] private float dimMaxDuration;
    [SerializeField] private float dimEndFade;

    private void Awake()
    {
        Instance = this;
    }

    public void ResultScreenButtonsSetActive(bool value) => resultScreenButtons.SetActive(value);

    public void ResultQuitConfirmationButtonsSetActive(bool value) => resultQuitConfirmation.SetActive(value);

    public void GameEnd(bool winner)
    {
        escMenuManager.gameObject.SetActive(false);
        RayCaster.Instance.eventsOn = false;
        StartCoroutine(DimAnimation(winner));
    }

    private IEnumerator DimAnimation(bool winner)
    {
        panelDim.enabled = true;
        float time = 0;
        while (time < dimMaxFade)
        {
            time += Time.deltaTime;
            float dim = Mathf.Lerp(0, dimMax, time / dimMaxFade);
            panelDim.color = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(dim));
            yield return null;
        }
        if (winner)
        {
            victoryEffect.SetActive(true);
            victoryEffect.GetComponent<GameOverEffectManager>().StartAnimation();
        }
        else
        {
            defeatEffect.SetActive(true);
            defeatEffect.GetComponent<GameOverEffectManager>().StartAnimation();
        }
        yield return new WaitForSeconds(dimMaxDuration);
        resultScreenButtons.SetActive(true);
        time = 0;
        while(time < dimEndFade)
        {
            time += Time.deltaTime;
            float dim = Mathf.Lerp(dimMax, dimEnd, time / dimEndFade);
            panelDim.color = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(dim));
            yield return null;
        }
    }

    [Button]
    public void AnimationDevButton()
    {
        resultScreenButtons.SetActive(false);
        StartCoroutine(DimAnimation(true));
    }
}
