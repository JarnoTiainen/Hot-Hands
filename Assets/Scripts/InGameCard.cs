using UnityEngine;
using TMPro;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] public CardData cardData;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;

    [SerializeField] private Shader cardMainBodyMaterial;
    private Material mat;


    private void Awake()
    {
        mat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.shader = cardMainBodyMaterial;
    }


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

    public void Burn()
    {
        Debug.Log(cardData.cardName + " was burned");
        GetComponent<DissolveEffect>().StartDissolving(mat);
        GetComponent<DissolveShadow>().StartDissolving();
        transform.GetChild(1).gameObject.SetActive(false);
        //transform.GetChild(0).GetComponent<MeshRenderer>().shadowCastingMode = 0;
    }

    public void Kill()
    {
        Debug.Log(cardData.cardName + " was killed");
    }
}
