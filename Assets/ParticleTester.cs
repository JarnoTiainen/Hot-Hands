using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ParticleTester : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [Button] public void InstantiateParticle()
    {
        GameObject newPrefab = Instantiate(prefab);
        newPrefab.transform.SetParent(gameObject.transform);
        newPrefab.transform.localPosition = Vector3.zero;

    }
}
