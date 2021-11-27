using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnpileManager : MonoBehaviour
{
    [SerializeField] private int owener;
    [SerializeField] [Range(0, 1)] private float visibility;
    [SerializeField] private float zlow;
    [SerializeField] private GameObject burnpile;


    void Update()
    {
        burnpile.transform.localPosition = new Vector3(0, 0, zlow * (1-visibility));
    }
}
