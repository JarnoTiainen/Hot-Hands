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
    [SerializeField] private CardBurn cardBurn;
    [SerializeField] private bool canAffordBool;

    [ShowIf("debuggerModeOn", true)] public int indexOnField;
    [ShowIf("debuggerModeOn", true)] public int serverConfirmedIndex;
    public bool confirmedByServer;

    [SerializeField] [ShowIf("debuggerModeOn", true)] public bool isGhostCard;
    [ShowIf("debuggerModeOn", true)] public bool cardHidden;

    [SerializeField] private float currentAttackCoolDown;
    [SerializeField] private bool attackOnCD;
    [SerializeField] private bool preAttackOnCD;


    private void Awake()
    {
        mat = meshRenderer.material;
        meshRenderer.material.shader = cardMainBodyMaterial;
    }

    [Button] public void StartAttackCooldown(float duration, bool isSummonCall = false)
    {
        attackOnCD = true;
        currentAttackCoolDown = duration;
    }

    private void Update()
    {
        if (currentAttackCoolDown > 0) currentAttackCoolDown -= Time.deltaTime;
        else if(currentAttackCoolDown <= 0 && attackOnCD)
        {
            ToggleAttackBurnEffect(true);
            attackOnCD = false;
            preAttackOnCD = false;
            currentAttackCoolDown = 0;
        }
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
        if(!attackOnCD && !preAttackOnCD)
        {
            preAttackOnCD = true;
            ToggleAttackBurnEffect(false);
            transform.parent.GetComponent<MonsterZone>().AttackWithCard(gameObject);
        }
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

    public void ToggleAttackBurnEffect(bool attackReady)
    {
        if(attackReady)
        {
            cardBurn.StartBurning();
        }
        else
        {
            cardBurn.EndBurning();
        }
    }

    public void ToggleCanAffordEffect(bool canAfford)
    {
        Debug.Log(canAfford + " " + cardData.cardName);
        if (canAfford && !canAffordBool)
        {
            canAffordBool = true;
            cardBurn.StartCanAfford();
        }
        else if(canAffordBool && !canAfford)
        {
            canAffordBool = false;
            cardBurn.EndCanAfford();
        }
    }
}
