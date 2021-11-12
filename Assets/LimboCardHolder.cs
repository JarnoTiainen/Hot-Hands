using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimboCardHolder : MonoBehaviour
{
    public static LimboCardHolder Instance { get; private set; }


    private void Awake()
    {
        Instance = gameObject.GetComponent<LimboCardHolder>();
    }

    public void StoreNewCard(GameObject card)
    {
        card.transform.SetParent(gameObject.transform);
        card.transform.localPosition = Vector3.zero;
    }
}
