using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultScript : MonoBehaviour
{
    public static MatchResultScript Instance { get; private set; }
    [SerializeField] private GameObject resultScreenButtons;

    private void Awake()
    {
        Instance = this;
    }

    public void ResultScreenButtonsSetActive(bool value)
    {
        resultScreenButtons.SetActive(value);
    }

}
