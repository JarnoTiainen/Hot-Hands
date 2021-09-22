using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update

    

    void Start()
    {
        GameEventManager.CallintThisMethod();
        Tooltip.ShowTooltip_Static("This is testrun for tooltips <sdf <dsf ffsdf <sdfeeee refds< d<f<<<<<<<<<< asd asd sdsa asd asd asd  sdsdsds sd sdasd <<<<<<<< dfdsf   asdas asd asd  asdaa dsdsd asd asd asd das asdas asdasda asdas aasdasda  asda asda aasdas  sdfsdfs      sdfsdffefes sdfww.", "now also with Title!");
        Tooltip.ShowTooltip_Static("This is tooltip2 text hehe!", "otsikko");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
