using UnityEngine;
using TMPro;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] public CardData cardData;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro cost;
    [SerializeField] private TextMeshPro value;
    [SerializeField] private TextMeshPro lp;
    [SerializeField] private TextMeshPro rp;

    public void SetNewCardData(bool isYourCard, CardData cardData)
    {
        this.cardData = cardData;
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();

        if(isYourCard)
        {
            if (value != null) lp.text = cardData.lp.ToString();
            if (value != null) rp.text = cardData.rp.ToString();
        }
        else
        {
            if (value != null) lp.text = cardData.rp.ToString();
            if (value != null) rp.text = cardData.lp.ToString();
        }
        
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void OnClickElement()
    {
        transform.parent.GetComponent<MonsterZone>().AttackWithCard(gameObject);
    }

    public void SetStatLp(int lp)
    {
        this.lp.text = lp.ToString();
    }
    public void SetStatRp(int rp)
    {
        this.rp.text = rp.ToString();
    }
}
