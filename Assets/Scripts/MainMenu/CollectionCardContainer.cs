using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionCardContainer : MonoBehaviour
{
    public Card card;
    [SerializeField] private GameObject card3DPrefab;

    public void InstantiateCard()
    {
        GameObject card3D = Instantiate(card3DPrefab);
        card3D.SetActive(true);
        CollectionCard3D collectionCard3D = card3D.GetComponent<CollectionCard3D>();
        collectionCard3D.card = card;
        collectionCard3D.Initialize();
        card3D.transform.SetParent(gameObject.transform, false);
    }
}
