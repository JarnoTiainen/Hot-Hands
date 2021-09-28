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

    [SerializeField]
    private bool showSettings = false;

    [ShowIf("showSettings")]
    [SerializeField]
    private bool editSettings = false;

    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private SoundtrackClip _soundtrackBase;

    [Title("Audio Source")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool waitToPlay = true;

    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool useDefault = true;

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


    private void SelectMusic()
    {
        UnityEditor.Selection.activeObject = soundtrackToPlay;
    }

    //Get's list of Soundtracks from manager, used in the inspector
    private List<SoundtrackClip> SoundtrackType()
    {
        List<SoundtrackClip> musicList;

        switch (soundtrackType)
        {
            case SoundtrackManager.SoundtrackType.Menu:
                musicList = SoundtrackManager.instance.menuSoundtrack;
                break;
            case SoundtrackManager.SoundtrackType.InGame:
                musicList = SoundtrackManager.instance.inGameSoundtrack;
                break;
            case SoundtrackManager.SoundtrackType.ResultScreen:
                musicList = SoundtrackManager.instance.resultScreenSoundtrack;
                break;
            default:
                musicList = SoundtrackManager.instance.menuSoundtrack;
                break;
        }

        return musicList;
    }

    public void PlaySoundtrack()
    {
        if (useDefault || audiosource == null)
            SoundtrackManager.PlaySoundtrack(soundtrackToPlay, waitToPlay, null);
        else
            SoundtrackManager.PlaySoundtrack(soundtrackToPlay, waitToPlay, audiosource);
    }
}
