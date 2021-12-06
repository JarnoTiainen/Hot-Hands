using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerChecks : MonoBehaviour
{
    public GameObject soundtrackManager;
    void Start()
    {
        if (FindObjectOfType<SoundtrackManager>()) return;

        else
        {
            GameObject newSoundtrackManager = Instantiate(soundtrackManager, new Vector3(0, 0, 0), Quaternion.identity);
            newSoundtrackManager.name = "SoundtrackManager";
            Debug.Log("Created new SoundtrackManager");
        }
    }
}
