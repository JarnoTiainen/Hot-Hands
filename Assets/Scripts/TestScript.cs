using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class TestScript : MonoBehaviour
{

    void Start()
    {
        Tooltip.ShowTooltip_Static("This isd  asdaa fsdfs      sdfsdffefes sdfww.", "now also with Title!");
        Tooltip.ShowTooltip_Static("This is tooltip2 text hehe!", "otsikko");
        Subscribe();
        GameEventManager.RunTestEvent();
    }

    private void Subscribe()
    {
        GameEventManager.OnRunTestEvent += TestEvent;
    }
    private void Unsubscribe()
    {
        GameEventManager.OnRunTestEvent -= TestEvent;
    }

    public void TestEvent()
    {
        Debug.Log("Test event successfull");
    }
    private void OnDisable()
    {
        Unsubscribe();
    }
}
