using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private float secondsBeforeStart;
    [SerializeField] private bool gameStarted;
    [SerializeField] private bool cardDrawDone;
    [SerializeField] private TextMeshProUGUI textCountdownNumber;
    [SerializeField] private Image panelImage;
    [SerializeField] private int dimStart;
    [SerializeField] private int dimEnd;


    private void Start()
    {
        RayCaster.Instance.eventsOn = false;
        StartCoroutine(DimAnimation());
    }

    void Update()
    {
        secondsBeforeStart -= Time.deltaTime;
        textCountdownNumber.text = ((int)secondsBeforeStart).ToString();
        if(secondsBeforeStart < 1 && !cardDrawDone)
        {
            WebSocketService.StartGame();
            cardDrawDone = true;
        }
        if(secondsBeforeStart < 0 && !gameStarted)
        {
            gameStarted = true;
            transform.parent.gameObject.SetActive(false);
            RayCaster.Instance.eventsOn = true;
        }
    }

    private IEnumerator DimAnimation()
    {
        float time = 0;
        float duration = secondsBeforeStart;
        int secondChange = (int)secondsBeforeStart;
        while (time < duration)
        {
            time += Time.deltaTime;
            float dim = Mathf.Lerp(dimStart, dimEnd, time / duration);
            panelImage.color = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(dim));
            if (secondChange != (int)time)
            {
                secondChange = (int)time;
                SFXLibrary.Instance.countdown.PlaySFX();
            }
            yield return null;
        }
    }
}
