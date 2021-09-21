using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update

    

    void Start()
    {
        GameEventManager.CallintThisMethod();
        Tooltip.ShowTooltip_Static("This is testrun for tooltips.", "It WORKS!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
