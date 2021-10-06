using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXLibrary : MonoBehaviour
{
    public static SFXLibrary Instance { get; private set; }

    private void Awake()
    {
        Instance = gameObject.GetComponent<SFXLibrary>();
    }


}
