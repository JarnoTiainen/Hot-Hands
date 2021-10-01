using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusicPlayer : MonoBehaviour
{
    public Soundtrack mainMenuSoundtrack;

    void Start()
    {
        mainMenuSoundtrack.PlaySoundtrack();
    }


}
