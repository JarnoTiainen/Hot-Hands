using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] private CardData cardData;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;
    public float fadeDuration = 0.5f;
    public Slider coolDownSlider;
    public bool isInHand;

    [SerializeField] private bool debuggerModeOn = false;
    [SerializeField] private Shader cardMainBodyMaterial;
    [SerializeField] private Shader cardImageShader;
    [SerializeField] private Material mat;
    [SerializeField] private Texture2D cardImage;
    [SerializeField] private Texture2D lpImage;
    [SerializeField] private Texture2D rpImage;
    [SerializeField] private Texture2D valueImage;
    [SerializeField] private MeshRenderer meshRendererBorderLow;
    [SerializeField] private MeshRenderer meshRenderercardBackLow;
    [SerializeField] private MeshRenderer meshRendererIconZoneLow;
    [SerializeField] private MeshRenderer meshRendererNameZoneLow;
    [SerializeField] private MeshRenderer meshRendererImage;
    [SerializeField] private MeshRenderer meshRendererImageLow;
    [SerializeField] public Canvas textCanvas;
    [SerializeField] private CardBurn cardBurn;
    [SerializeField] private bool canAffordBool;
    public bool interActable = true;
    [SerializeField] private GameObject takeDamageEffectPrefab;
    [SerializeField] private DescriptionLogoManager descriptionLogoManager;
    [SerializeField] private GameObject leftAttackSymbol;
    [SerializeField] private GameObject rigthAttackSymbol;
    [SerializeField] private GameObject leftDefenceSymbol;
    [SerializeField] private GameObject rightDefenceSymbol;

    [ShowIf("debuggerModeOn", true)] public int serverConfirmedIndex;
    public bool confirmedByServer;

    [SerializeField] [ShowIf("debuggerModeOn", true)] public bool isGhostCard;
    [ShowIf("debuggerModeOn", true)] public bool cardHidden;

    private float maxAttackCoolDown;
    [SerializeField] private float currentAttackCoolDown;
    [SerializeField] private bool attackOnCD;
    [SerializeField] private bool preAttackOnCD;
    public bool showTooltips;
    public int owner;
    private bool doOnce;

    [HideInInspector] public int tempRp;
    [HideInInspector] public int tempLp;

    [SerializeField] private CardRuneEffectManager cardRuneEffectManager;

    public void SetTempValuesAsValues()
    {
        UpdateRPLP(tempRp, tempLp);
    }

    private void Awake()
    {
        mat = meshRendererBorderLow.material;
        meshRendererImage.material.shader = cardImageShader;
        
        meshRendererImageLow.material.shader = cardMainBodyMaterial;
        meshRendererBorderLow.material.shader = cardMainBodyMaterial;
        meshRenderercardBackLow.material.shader = cardMainBodyMaterial;
        meshRendererIconZoneLow.material.shader = cardMainBodyMaterial;
        meshRendererNameZoneLow.material.shader = cardMainBodyMaterial;
        coolDownSlider.gameObject.SetActive(false);


        /* meshRendererValue.material.renderQueue = 3100;
        meshRendererLP.material.renderQueue = 3100;
        meshRendererRP.material.renderQueue = 3100;
        meshRendererImageLow.material.renderQueue = 3000;
        meshRendererBorderLow.material.renderQueue = 3000;
        meshRenderercardBackLow.material.renderQueue = 3000;
        meshRendererIconZoneLow.material.renderQueue = 2900;
        meshRendererNameZoneLow.material.renderQueue = 2900;
        meshRendererImage.material.renderQueue = 3100; */
        
    }

    [Button] public void StartAttackCooldown(float duration, bool isSummonCall = false)
    {
        attackOnCD = true;
        doOnce = true;
        currentAttackCoolDown = duration;
        maxAttackCoolDown = duration;
    }

    public void UpdateRPLP(int rp, int lp)
    {
        Debug.Log("Updating rp and lp");

        if(rp < cardData.rp && lp <= cardData.lp || rp <= cardData.rp && lp < cardData.lp) 
        {
            GameObject newTakeDamageEffect = Instantiate(takeDamageEffectPrefab);
            newTakeDamageEffect.transform.SetParent(transform);
            newTakeDamageEffect.transform.localPosition = new Vector3(0, 0, -0.03f);
        }
        cardData.rp = rp;
        cardData.lp = lp;
    }
    public CardData GetData()
    {
        return cardData;
    }
 
    private void Update()
    {
        if (currentAttackCoolDown > 0) {
            currentAttackCoolDown -= Time.deltaTime;

            //if the card stopped attacking, show the cooldownbar
            if (!(GetComponent<CardMovement>().doAttack) && (GetComponent<CardMovement>().endPoint.y == transform.localPosition.y)) {
                if (doOnce) {
                    maxAttackCoolDown = currentAttackCoolDown;
                    coolDownSlider.gameObject.SetActive(true);
                    coolDownSlider.GetComponent<CanvasGroup>().alpha = 0;
                    coolDownSlider.value = 1;
                    StartCoroutine(Fade(fadeDuration, true));                   //fade in
                    doOnce = false;
                }
                coolDownSlider.value = currentAttackCoolDown / maxAttackCoolDown;

                //fade out
                if (currentAttackCoolDown <= fadeDuration) {
                    StartCoroutine(Fade(fadeDuration, false));
                }

            }
        }
        else if(currentAttackCoolDown <= 0 && attackOnCD)
        {
            coolDownSlider.gameObject.SetActive(false);
            ToggleAttackBurnEffect(true);
            attackOnCD = false;
            preAttackOnCD = false;
            currentAttackCoolDown = 0;
        }

        meshRendererImage.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
        meshRendererImageLow.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
        meshRendererBorderLow.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
        meshRenderercardBackLow.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
        meshRendererNameZoneLow.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
        meshRendererIconZoneLow.material.SetFloat("_DissolveAmount", mat.GetFloat("_DissolveAmount"));
    }

    /// <summary>
    /// Fades the cooldown bar in or out depending on isFadeIn boolean
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="isFadeIn"></param>
    /// <returns></returns>
    private IEnumerator Fade(float duration, bool isFadeIn)
    {
        if(isFadeIn) {
            //fade in
            float elapsedTime = Time.deltaTime;
            while (coolDownSlider.GetComponent<CanvasGroup>().alpha != 1)
            {
                coolDownSlider.GetComponent<CanvasGroup>().alpha = Mathf.Clamp(1 - ((duration - elapsedTime) / duration), 0, 1);
                elapsedTime += Time.deltaTime;
                yield return 0;
            }
        } else {
            //fade out
            float elapsedTime = duration;
            while (coolDownSlider.GetComponent<CanvasGroup>().alpha != 0)
            {
                coolDownSlider.GetComponent<CanvasGroup>().alpha = Mathf.Clamp(1 - ((duration - elapsedTime) / duration), 0, 1);
                elapsedTime -= Time.deltaTime;
                yield return 0;
            }
        }
    }

    public void SetNewCardData(bool isYourCard, CardData cardData)
    {
        Debug.Log("Setting new card data");

        this.cardData = cardData;
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();

        meshRendererImage.material.SetTexture("_CardImage", cardData.cardSprite.texture);
        UpdateRPLP(cardData.rp, cardData.lp);


        if (isYourCard)
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
        if(!cardHidden && showTooltips)
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
        if (!Mouse.Instance.targetModeOn && interActable)
        {
            if (!attackOnCD && !preAttackOnCD && GameManager.Instance.IsYou(owner))
            {
                preAttackOnCD = true;
                ToggleAttackBurnEffect(false);
                if (GameManager.Instance.IsYou(owner)) WebSocketService.Attack(cardData.seed);
            }
        }
        else
        {
            GameManager.Instance.EndTargetEvent(cardData.seed);
        }
    }
    public void UpdateCardTexts()
    {
        this.lp.text = cardData.lp.ToString();
        this.rp.text = cardData.rp.ToString();
    }

    public void Burn()
    {
        ToggleCanAffordEffect(false);
        GetComponent<DissolveEffect>().StartDissolving(mat);
        GetComponent<DissolveMove>().StartDissolving();
        transform.GetChild(1).gameObject.SetActive(false);
    }

    [Button] public void SpellBurn()
    {
        ToggleCanAffordEffect(false);
        cardRuneEffectManager.PlayRuneEffect();
    }

    [Button]
    public void ReverseSpellBurn()
    {
        ToggleCanAffordEffect(true);
        cardRuneEffectManager.PlayReverseRuneEffect();
    }

    public void ToggleGhostCard(bool on)
    {
        if(on)
        {
            isGhostCard = true;
            //you can use just this to hide the whole gameobject
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            meshRendererBorderLow.enabled = false;
            meshRenderercardBackLow.enabled = false;
            meshRendererIconZoneLow.enabled = false;
            meshRendererNameZoneLow.enabled = false;
            textCanvas.enabled = false;
            cardHidden = true;
        }
        else
        {
            isGhostCard = false;
            
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            meshRendererBorderLow.enabled = true;
            meshRenderercardBackLow.enabled = true;
            meshRendererIconZoneLow.enabled = true;
            meshRendererNameZoneLow.enabled = true;
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
        if(debuggerModeOn) Debug.Log(canAfford + " " + cardData.cardName);
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

    public void StartDestructionEvent()
    {
        interActable = false;
        References.i.opponentMonsterZone.TryRemoveMonsterCard(cardData.seed);
        References.i.yourMonsterZone.TryRemoveMonsterCard(cardData.seed);
        //TODO: play animation here before destroying the card
        Destroy(gameObject);
    }

    public void SetDescription()
    {
        descriptionLogoManager.SetNewImage(cardData.enchantments);
        descriptionText.text = cardData.description;


    }

    public void SetAttackDirectionSymbol()
    {
        Debug.Log("Owenr " + owner);
        if(cardData.attackDirection == Card.AttackDirection.Left)
        {
            if(GameManager.Instance.IsYou(owner))
            {
                leftAttackSymbol.SetActive(true);
                rightDefenceSymbol.SetActive(true);
            }
            else
            {
                rigthAttackSymbol.SetActive(true);
                leftDefenceSymbol.SetActive(true);
            }
        }
        else if(cardData.attackDirection == Card.AttackDirection.Right)
        {
            if (GameManager.Instance.IsYou(owner))
            {
                rigthAttackSymbol.SetActive(true);
                leftDefenceSymbol.SetActive(true);
            }
            else
            {
                leftAttackSymbol.SetActive(true);
                rightDefenceSymbol.SetActive(true);
            }
        }
        if(cardData.cardType == Card.CardType.Spell)
        {
            rp.gameObject.SetActive(false);
            lp.gameObject.SetActive(false);
        }
    }
}
