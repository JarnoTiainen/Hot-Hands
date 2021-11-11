using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortMethodDropdownButton : MonoBehaviour
{
    [SerializeField] private GameObject sortMethodDropdown;

    public void ToggleDropdown()
    {
        sortMethodDropdown.SetActive(!sortMethodDropdown.activeSelf);
    }
}
