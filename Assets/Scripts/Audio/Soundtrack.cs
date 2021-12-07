using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Soundtrack
{
    [LabelText("Soundtrack Type")]
    [LabelWidth(100)]
    [OnValueChanged("SoundtrackChange")]
    [InlineButton("PlaySoundtrack")]
    public SoundtrackManager.SoundtrackType soundtrackType = SoundtrackManager.SoundtrackType.Menu;

    [LabelText("$soundtrackLabel")]
    [LabelWidth(100)]
    [ValueDropdown("SoundtrackType")]
    [OnValueChanged("SoundtrackChange")]
    [InlineButton("SelectSoundtrack")]
    public SoundtrackClip soundtrackToPlay;
    private string soundtrackLabel = "Soundtrack";

#pragma warning disable 0414
    [SerializeField]
    private bool showSettings = false;

    [ShowIf("showSettings")]
    [SerializeField]
    private bool editSettings = false;
#pragma warning restore 0414

    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private SoundtrackClip _soundtrackBase;

    [Title("Audio Source")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool waitToPlay = false;

    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool loop = true;

    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool useDefault = false;

    [DisableIf("useDefault")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private AudioSource audiosource;

    private void SoundtrackChange()
    {
        //keep the label up to date
        soundtrackLabel = soundtrackType.ToString() + " Soundtrack";

        //keep the displayed "Soundtrack clip" up to date
        _soundtrackBase = soundtrackToPlay;
    }

    private void SelectSoundtrack()
    {
        // Platform #define directives for UnityEditor
        #if UNITY_EDITOR
        UnityEditor.Selection.activeObject = soundtrackToPlay;
        #endif

    }

    //Get's list of Soundtracks from manager, used in the inspector
    private List<SoundtrackClip> SoundtrackType()
    {
        List<SoundtrackClip> musicList;

        switch (soundtrackType)
        {
            case SoundtrackManager.SoundtrackType.Menu:
                musicList = SoundtrackManager.Instance.menuSoundtrack;
                break;
            case SoundtrackManager.SoundtrackType.InGame:
                musicList = SoundtrackManager.Instance.inGameSoundtrack;
                break;
            default:
                musicList = SoundtrackManager.Instance.menuSoundtrack;
                break;
        }

        return musicList;
    }

    public void PlaySoundtrack()
    {
        if (useDefault || audiosource == null)
            SoundtrackManager.PlaySoundtrack(soundtrackToPlay, waitToPlay, loop, useDefault, null);
        else
            SoundtrackManager.PlaySoundtrack(soundtrackToPlay, waitToPlay, loop, useDefault, audiosource);
    }
}
