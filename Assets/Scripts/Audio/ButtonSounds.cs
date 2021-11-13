using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour
{
    private static GameObject sfxLibrary;
    [SerializeField] bool clearTriggersOnDisable = true;

    private void Start()
    {
        sfxLibrary = GameObject.Find("SFXLibrary");
        gameObject.GetComponent<Button>().onClick.AddListener(() => OnClick());

        if (gameObject.GetComponent<EventTrigger>() == null) return;
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => OnHover());
        trigger.triggers.Add(entry);
    }

    private void OnEnable()
    {
        if (!clearTriggersOnDisable) return;
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
        if (!clearTriggersOnDisable) return;
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        if (gameObject.GetComponent<EventTrigger>() == null) return;
        gameObject.GetComponent<EventTrigger>().triggers.Clear();
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        if (gameObject.GetComponent<EventTrigger>() == null) return;
        gameObject.GetComponent<EventTrigger>().triggers.Clear();
    }

    public void OnHover()
    {
        sfxLibrary.GetComponent<ButtonSFX>().OnHover();
    }

    public void OnClick()
    {
        sfxLibrary.GetComponent<ButtonSFX>().OnClick();
    }


}
