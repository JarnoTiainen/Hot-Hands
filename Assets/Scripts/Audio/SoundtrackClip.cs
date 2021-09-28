using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "New Music Clip", fileName = "NewMusicClip")]
public class SoundtrackClip : ScriptableObject
{
    [Space]
    [Title("Music Clip")]
    [Required]
    public AudioClip clip;

    [Title("Clip Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 2f)]
    public float pitch = 1f;
}
