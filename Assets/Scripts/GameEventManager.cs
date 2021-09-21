using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set;}

    public static void CallintThisMethod()
    {
        Debug.Log("it works");
    }
}
