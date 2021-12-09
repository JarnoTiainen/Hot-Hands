using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatcodeManager : MonoBehaviour
{
    [SerializeField] private AdminControls adminControls;
    private string adminCode = "SCHOB";
    private string bufferString = "";
    private float timeCutoff = 1f;
    private float timer;
    private bool enabled = false;

    private void Start()
    {
        timer = timeCutoff;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) bufferString = "";
        if (bufferString.Length == adminCode.Length && !enabled) CheckCode();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            timer = timeCutoff;
            bufferString += e.keyCode.ToString();
        }
    }

    private void CheckCode()
    {
        if (bufferString.EndsWith(adminCode))
        {
            MainMenu.Instance.CreatePopupNotification("Admin mode enabled", MainMenu.PopupCorner.BottomLeft, MainMenu.PopupTone.Positive);
            timer = 0;
            adminControls.gameObject.SetActive(true);
            adminControls.EnableAdminFeatures();
            enabled = true;
        }
    }
}
