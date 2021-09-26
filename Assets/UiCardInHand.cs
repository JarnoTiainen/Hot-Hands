using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCardInHand : MonoBehaviour
{
    public bool mouseOverElement = false;

    public void OnMouseOver()
    {
        if (!mouseOverElement) {
            ScaleCardUp();
            mouseOverElement = true;
        }
    }

    public void OnMouseExit()
    {
        if (mouseOverElement)
        {
            ScaleCardDown();
            mouseOverElement = false;
        }
    }

    public void ScaleCardUp()
    {
        transform.parent.parent.GetComponent<UiHand>().IncreaseCardSize(transform.parent.gameObject);
    }
    public void ScaleCardDown()
    {
        transform.parent.parent.GetComponent<UiHand>().DecreaseCardSize(transform.parent.gameObject);
    }
}
