using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortMethodToggle : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            ChangeSortMethod();
        });
    }

    private void ChangeSortMethod()
    {
        if(gameObject.GetComponent<Toggle>().isOn) SortMethodDropdown.Instance.SortAllLists();
    }
}
