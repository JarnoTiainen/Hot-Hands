using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private float secondsBeforeStart;
    [SerializeField] private bool gameStarted;

    [SerializeField] private TextMeshProUGUI textCountdownNumber;

    private void Start()
    {
        RayCaster.Instance.eventsOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        secondsBeforeStart -= Time.deltaTime;
        textCountdownNumber.text = ((int)secondsBeforeStart).ToString();
        if(secondsBeforeStart < 0 && !gameStarted)
        {
            WebSocketService.StartGame();
            gameStarted = true;
            transform.parent.gameObject.SetActive(false);
            RayCaster.Instance.eventsOn = true;
        }
    }
}
