using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class RayCaster : MonoBehaviour
{
    [SerializeField] private Camera rayCamera;
    public GameObject target;
    public GameObject newTarget;
    public GameObject previousTarget;
    public bool firstTimeCall = true;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if(target)
            {
                
                newTarget = hit.transform.gameObject;
                if (target != hit.transform.gameObject)
                {
                    NewTarget(target, hit.transform.gameObject);
                }
            }
            target = hit.transform.gameObject;


            if (Input.GetMouseButtonDown(0))
            {
                if (target.GetComponent<IOnClickDownUIElement>() != null) target.GetComponent<IOnClickDownUIElement>().OnClickElement();
            }
            if (target.GetComponent<IOnHoverUIElement>() != null) target.GetComponent<IOnHoverUIElement>().OnHoverElement();
        }
        else
        {
            if (!previousTarget && target)
            {
                previousTarget = target;
                target = null;
            }
            if (previousTarget)
            {
                if (previousTarget.GetComponent<IOnHoverExitElement>() != null) previousTarget.GetComponent<IOnHoverExitElement>().OnHoverExit();
                previousTarget = null;
            }
        }
    }

    private void NewTarget(GameObject oldTarget, GameObject newTarget)
    {
        if (newTarget.GetComponent<IOnHoverEnterElement>() != null)
        {
            newTarget.GetComponent<IOnHoverEnterElement>().OnHoverEnter();
        }
        Debug.Log("target 2 " + target);
        if (oldTarget)
        {
            if (oldTarget.GetComponent<IOnHoverExitElement>() != null) oldTarget.GetComponent<IOnHoverExitElement>().OnHoverExit();
        }
    }
}
