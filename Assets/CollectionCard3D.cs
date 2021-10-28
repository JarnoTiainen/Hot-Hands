using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CollectionCard3D : MonoBehaviour
{
    public Card card;
    [SerializeField]

    private void Start()
    {
        Transform cardCanvas = gameObject.transform.Find("Canvas");
        cardCanvas.Find("CardName").GetComponent<TextMeshProUGUI>().text = card.cardName;
        cardCanvas.Find("Cost").GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        cardCanvas.Find("RP").GetComponent<TextMeshProUGUI>().text = card.rp.ToString();
        cardCanvas.Find("LP").GetComponent<TextMeshProUGUI>().text = card.lp.ToString();
        cardCanvas.Find("Value").GetComponent<TextMeshProUGUI>().text = card.value.ToString();
    }

    public void AddCard()
    {
        GameObject.FindGameObjectWithTag("DeckBuild").GetComponent<DeckBuild>().AddCard(card);
    }


}
