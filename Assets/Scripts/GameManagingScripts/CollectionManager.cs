using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    private CardList cardList;

    public GameObject cardListGameObject;
    public GameObject cardContainerPrefab;



    void Start()
    {
        cardList = Resources.Load("Card List") as CardList;



        foreach(CardList.ListCard listCard in cardList.allCards)
        {
            GameObject newContainer = Instantiate(cardContainerPrefab) as GameObject;
            newContainer.SetActive(true);

            Transform newCard = newContainer.transform.Find("Card");

            newCard.Find("Name").GetComponent<TextMeshProUGUI>().text = listCard.name;
            newCard.Find("Cost").GetComponent<TextMeshProUGUI>().text = listCard.card.cost.ToString();
            newCard.Find("RP").GetComponent<TextMeshProUGUI>().text = listCard.card.rp.ToString();
            newCard.Find("LP").GetComponent<TextMeshProUGUI>().text = listCard.card.lp.ToString();
            newCard.Find("Value").GetComponent<TextMeshProUGUI>().text = listCard.card.value.ToString();


            newContainer.transform.SetParent(cardListGameObject.transform, false);
        }
    }



}
