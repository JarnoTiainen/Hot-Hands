using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] public CardData cardData;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;

    [SerializeField] private bool debuggerModeOn = false;
    [SerializeField] private Shader cardMainBodyMaterial;
    private Material mat;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] public Canvas textCanvas;
    [ShowIf("debuggerModeOn", true)] public int indexOnField;

    [SerializeField] [ShowIf("debuggerModeOn", true)] private bool isGhostCard;
    [ShowIf("debuggerModeOn", true)] public bool cardHidden;

    private void Awake()
    {
        mat = meshRenderer.material;
        meshRenderer.material.shader = cardMainBodyMaterial;
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

    public void OnHoverEnter()
    {
        if(!cardHidden)
        {
            UiCardPreviewManager.Instance.ShowCardTooltip(gameObject);
        }
    }

    public void OnHoverExit()
    {
        UiCardPreviewManager.Instance.HideCardTooltip(gameObject);
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
        GetComponent<DissolveMove>().StartDissolving();
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ToggleGhostCard()
    {
        if(!isGhostCard)
        {
            isGhostCard = true;
            meshRenderer.enabled = false;
            textCanvas.enabled = false;
            cardHidden = true;
        }
        else
        {
            isGhostCard = false;
            meshRenderer.enabled = true;
            textCanvas.enabled = true;
            cardHidden = false;
        }
    }
}
