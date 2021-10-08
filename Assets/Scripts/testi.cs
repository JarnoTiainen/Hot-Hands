using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testi : MonoBehaviour
{
    

    private void Start()
    {
        List < Card.MonsterTag > mtags = new List<Card.MonsterTag>();
        mtags.Add((Card.MonsterTag)0);
        mtags.Add((Card.MonsterTag)0);
        mtags.Add((Card.MonsterTag)1);        
    }
}
