using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable][CreateAssetMenu(fileName = "New Card", menuName = "Card/Empty Card")]
public class Dialogue : ScriptableObject
{
    public string name;
    public string[] sentences;
}
