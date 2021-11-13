using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ForceLayoutRebuild : MonoBehaviour
{
    [SerializeField] private GameObject togglesRow;

    // Needed to make the content size fitter component behave properly 
    private void OnEnable()
    {
        RectTransform togglesRectTransform = togglesRow.transform.parent.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(togglesRectTransform);
    }
}
