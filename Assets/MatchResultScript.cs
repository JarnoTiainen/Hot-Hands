using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MatchResultScript : MonoBehaviour
{
    public static MatchResultScript Instance { get; private set; }
    [SerializeField] private GameObject resultScreenButtons;
    [SerializeField] private Image panelDim;
    [SerializeField] private TextMeshProUGUI resultText;
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

    public void GameEnd(bool winner)
    {
        if (winner) resultText.text = "VICTORY";
        else resultText.text = "DEFEAT";
        StartCoroutine(DimAnimation());
    }

    private IEnumerator DimAnimation()
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
        resultText.gameObject.SetActive(true);
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
        resultText.gameObject.SetActive(false);
        resultScreenButtons.SetActive(false);
        StartCoroutine(DimAnimation());
    }
}
