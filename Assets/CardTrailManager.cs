using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardTrailManager : MonoBehaviour
{
    [SerializeField] private GameObject trail1;
    [SerializeField] private GameObject trail2;
    [SerializeField] private GameObject trail3;


    public void DisableTrails()
    {
        trail1.SetActive(false);
        trail2.SetActive(false);
        trail3.SetActive(false);

        Debug.Log("disabling");
    }

    public void EnableTrails()
    {
        trail1.SetActive(true);
        trail2.SetActive(true);
        trail3.SetActive(true);

        Debug.Log("enabling");
    }
}
