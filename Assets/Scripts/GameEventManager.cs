using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set;}
    public static int playerNumber;

    //events here. Always start with "On" keyword
    public static event Action OnRunTestEvent;
    public static event Action<int> OnPlayerDrawCard;
    
    //needs reference for slot, and what card is summoned
    public static event Action OnPlayMonster;

    public static void RunTestEvent()
    {
        OnRunTestEvent?.Invoke();
    }
    public static void PlayerDrawCard(int player)
    {
        OnPlayerDrawCard?.Invoke(player);
    }
    public static void PlayMonster()
    {
        OnPlayMonster?.Invoke();
    }
}
