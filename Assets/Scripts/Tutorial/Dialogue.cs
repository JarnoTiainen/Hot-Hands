using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable] [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : MonoBehaviour
{
    public string speaker_name;

    [BoxGroup ("Split/Sentences")]
    [TextArea]
    public string[] sentences;

}
