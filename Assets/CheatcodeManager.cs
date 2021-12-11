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
    private bool adminModeEnabled = false;

    private void Start()
    {
        timer = timeCutoff;
    }

    private void Update()
    {
        // Checks if too much time has passed between keypresses and resets the buffer. Calls CheckCode if buffer matches cheatcode length
        timer -= Time.deltaTime;
        if (timer <= 0) bufferString = "";
        if (bufferString.Length == adminCode.Length && !adminModeEnabled) CheckCode();
    }

    void OnGUI()
    {
        // If a key is pressed, gets the corresponding character and adds it to the buffer and resets the timer
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
            adminModeEnabled = true;
        }
    }
}
