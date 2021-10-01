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
        DrawCardMessage drawCardMessage = new DrawCardMessage(0, "Dragon Warrior of Warrior", 1, 3, 1, mtags, new List<Card.SpellTag>(), 3, 5);
        
        Debug.Log(drawCardMessage.cardName);
        
    }
}
