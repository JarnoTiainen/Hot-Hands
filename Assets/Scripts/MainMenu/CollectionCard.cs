using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CollectionCard : MonoBehaviour
{
    public Card card;

    private void Start()
    {
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.cardName;
        gameObject.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        gameObject.transform.Find("RP").GetComponent<TextMeshProUGUI>().text = card.rp.ToString();
        gameObject.transform.Find("LP").GetComponent<TextMeshProUGUI>().text = card.lp.ToString();
        gameObject.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = card.value.ToString();
    }

    public void AddCard()
    {
        GameObject.FindGameObjectWithTag("DeckBuild").GetComponent<DeckBuild>().AddCard(card);
    }


}
