using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchingGamePopupScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    private bool searching = true;
    private int seconds = 0;
    private int minutes = 0;

    private void OnEnable()
    {
        timeText.text = "00:00";
        StartCoroutine(Timer());
    }

    private void OnDisable()
    {
        searching = false;
    }

    private IEnumerator Timer()
    {
        while (searching)
        {
            yield return new WaitForSeconds(1f);
            seconds++;

            if(seconds == 60)
            {
                minutes++;
                seconds = 0;
            }

            string minutesString;
            if(minutes >= 10) minutesString = minutes.ToString();
            else minutesString = "0" + minutes.ToString();

            string secondsString;
            if (seconds >= 10) secondsString = seconds.ToString();
            else secondsString = "0" + seconds.ToString();

            timeText.text = minutesString + ":" + secondsString;
        }
    }
}
