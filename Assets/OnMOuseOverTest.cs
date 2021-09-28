using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMOuseOverTest : MonoBehaviour, IOnHoverUIElement, IOnHoverExitElement, IOnHoverEnterElement
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseEnter()
    {
        Debug.Log("Mouse is over");
    }

    public void OnHoverElement()
    {
        
    }

    public void OnHoverExit()
    {
        Debug.Log("Exit hover");
    }

    public void OnHoverEnter()
    {
        Debug.Log("Enter hover");
    }
}
