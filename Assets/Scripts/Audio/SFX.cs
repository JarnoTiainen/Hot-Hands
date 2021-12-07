using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class SFX
{
    [LabelText("SFX Type")]
    [LabelWidth(100)]
    [OnValueChanged("SFXChange")]
    [InlineButton("PlaySFX")]
    public SFXManager.SFXType sfxType = SFXManager.SFXType.UI;

    [LabelText("$sfxLabel")]
    [LabelWidth(100)]
    [ValueDropdown("SFXType")]
    [OnValueChanged("SFXChange")]
    [InlineButton("SelectSFX")]
    public SFXClip sfxToPlay;
    private string sfxLabel = "SFX";

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
    private SFXClip _sfxBase;

    [Title("Audio Source")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool waitToPlay = false;

    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private bool useDefault = false;

    [DisableIf("useDefault")]
    [ShowIf("showSettings")]
    [EnableIf("editSettings")]
    [SerializeField]
    private AudioSource audiosource;

    private void SFXChange()
    {
        //keep the label up to date
        sfxLabel = sfxType.ToString() + " SFX";

        //keep the displayed "SFX clip" up to date
        _sfxBase = sfxToPlay;
    }


    private void SelectSFX()
    {
        // Platform #define directives for UnityEditor
        #if UNITY_EDITOR
        UnityEditor.Selection.activeObject = sfxToPlay;
        #endif
    }

    //Get's list of SFX from manager, used in the inspector
    private List<SFXClip> SFXType()
    {
        List<SFXClip> sfxList;

        switch (sfxType)
        {
            case SFXManager.SFXType.UI:
                sfxList = SFXManager.Instance.uiSFX;
                break;
            case SFXManager.SFXType.Ambient:
                sfxList = SFXManager.Instance.ambientSFX;
                break;
            case SFXManager.SFXType.Effects:
                sfxList = SFXManager.Instance.effectsSFX;
                break;
            default:
                sfxList = SFXManager.Instance.uiSFX;
                break;
        }

        return sfxList;
    }

    public void PlaySFX()
    {
        if (useDefault || audiosource == null)
            SFXManager.Instance.PlaySFX(sfxToPlay, waitToPlay, useDefault, null);
        else
            SFXManager.Instance.PlaySFX(sfxToPlay, waitToPlay, useDefault, audiosource);
    }

    public void PlayUISFX()
    {
        SFXManager.Instance.PlayUISFX(sfxToPlay);
    }
}
