using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
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
        StartCoroutine(FadeMixerGroup.StartFade(SoundtrackManager.masterMixer, "inGameMusicVol", 1f, 0.0001f));
        StartCoroutine(FadeMixerGroup.DestroySoundtrack(GameObject.Find("InGameSoundtrackPlayer"), 1f));
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
            SFXLibrary.Instance.victory.PlaySFX();
        }
        else
        {
            defeatEffect.SetActive(true);
            defeatEffect.GetComponent<GameOverEffectManager>().StartAnimation();
            SFXLibrary.Instance.defeat.PlaySFX();
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
