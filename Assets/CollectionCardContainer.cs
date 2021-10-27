using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionCardContainer : MonoBehaviour
{
    public Card card;
    [SerializeField]
    private GameObject card3DPrefab;

    void Start()
    {
        
    }

    public void InstantiateCard()
    {
        GameObject card3D = Instantiate(card3DPrefab) as GameObject;
        card3D.SetActive(true);

        card3D.GetComponent<CollectionCard3D>().card = card;

        card3D.transform.SetParent(gameObject.transform, false);

    }


}
