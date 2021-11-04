using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DecorativeCard : MonoBehaviour
{
    public Card card;
    [SerializeField] private MeshRenderer meshRendererImage;

    public void UpdateUI()
    {
        Transform cardCanvas = gameObject.transform.Find("Canvas");
        cardCanvas.Find("CardName").GetComponent<TextMeshProUGUI>().text = card.cardName;
        cardCanvas.Find("Cost").GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        cardCanvas.Find("RP").GetComponent<TextMeshProUGUI>().text = card.rp.ToString();
        cardCanvas.Find("LP").GetComponent<TextMeshProUGUI>().text = card.lp.ToString();
        cardCanvas.Find("Value").GetComponent<TextMeshProUGUI>().text = card.value.ToString();

        meshRendererImage.material.SetTexture("_CardImage", card.cardSprite.texture);
    }
}
