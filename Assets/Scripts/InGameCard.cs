using UnityEngine;
using TMPro;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] private CardData cardData;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro cost;
    [SerializeField] private TextMeshPro value;


    public void SetNewCardData(CardData cardData)
    {
        this.cardData = cardData;
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void OnClickElement()
    {
        transform.parent.GetComponent<MonsterZone>().AttackWithCard(gameObject);
    }
}
