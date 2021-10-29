using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour
{
    private static GameObject sfxLibrary;

    private void Start()
    {
        sfxLibrary = GameObject.Find("SFXLibrary");

    }

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => OnClick());

        if (gameObject.GetComponent<EventTrigger>() == null) return;
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => OnHover());
        trigger.triggers.Add(entry);
    }
    private void OnDisable()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        if (gameObject.GetComponent<EventTrigger>() == null) return;
        gameObject.GetComponent<EventTrigger>().triggers.Clear();
    }

    public void OnHover()
    {
        sfxLibrary.GetComponent<ButtonSFX>().buttonHover.PlaySFX();
    }

    public void OnClick()
    {
        sfxLibrary.GetComponent<ButtonSFX>().buttonClick.PlaySFX();
    }


}
