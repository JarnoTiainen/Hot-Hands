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
    [SerializeField] private Material cardMainBodyMaterial;
    [SerializeField] private Material spellardMainBodyMaterial;
    [SerializeField] private Material legendaryCardMainBodyMaterial;
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
    [SerializeField] private DescriptionLogoManager descriptionLogoManager;
    [SerializeField] private GameObject leftAttackSymbol;
    [SerializeField] private GameObject rigthAttackSymbol;
    [SerializeField] private GameObject leftDefenceSymbol;
    [SerializeField] private GameObject rightDefenceSymbol;
    [SerializeField] private CardTakeDamageManager cardTakeDamageManager;
    [SerializeField] private SpectralEffectManager spectralEffectManager;
    [SerializeField] private DebuffEffectManager debuffEffectManager;
    [SerializeField] private BuffEffectMaterial buffEffectManager;

    [ShowIf("debuggerModeOn", true)] public int serverConfirmedIndex;
    public bool confirmedByServer;

    [SerializeField] [ShowIf("debuggerModeOn", true)] public bool isGhostCard;
    [ShowIf("debuggerModeOn", true)] public bool cardHidden;

    private float maxAttackCoolDown;
    [SerializeField] private float currentAttackCoolDown;
    [SerializeField] private bool attackOnCD;
    [SerializeField] private bool preAttackOnCD;
    [SerializeField] private float timeOnPreAttack;
    public bool showTooltips;
    public int owner;
    private bool doOnce;

    [HideInInspector] public int tempRp;
    [HideInInspector] public int tempLp;

    [SerializeField] private CardRuneEffectManager cardRuneEffectManager;
    [SerializeField] private CardTrailManager cardTrailManager;
    [SerializeField] private DrawCardReadyManager drawCardReadyManager;
    [SerializeField] private TextMeshProUGUI enchantmentCountText;
    [SerializeField] private DrawCDEffectManager drawCDEffectManager;
    [SerializeField] private GameObject drawCDElement;

    public GameObject deckCardCountElement;

    private float timeAsUnhandledHandCard = 0;
    private bool unhandled;

    private float timeAsLimboSpell = 0;
    private bool inspellLimbo;

    private bool inSummonLimbo;
    private float timeInSummonLimbo;

    private bool inBurnLimbo;
    private float timeInBurnLimbo;

    [SerializeField] private bool cardInHand;

    [SerializeField] private DealDamageToPlayerParticleManager dealDamageToPlayerParticleManager;
    [SerializeField] private GameObject enchantmentDescriptionGameObject;

    public void SetTempValuesAsValues()
    {
        UpdateRPLP(tempRp, tempLp);
    }

    private void Awake()
    {
        SetCardMaterial();
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

    public void SetParticleTarget(bool isYourCard)
    {
        dealDamageToPlayerParticleManager.SetForceField(isYourCard);
    }

    public void SetCardMaterial()
    {
        if(cardData.cardType == Card.CardType.Monster)
        {
            meshRendererImageLow.material = cardMainBodyMaterial;
            meshRendererBorderLow.material = cardMainBodyMaterial;
            meshRenderercardBackLow.material = cardMainBodyMaterial;
            meshRendererIconZoneLow.material = cardMainBodyMaterial;
            meshRendererNameZoneLow.material = cardMainBodyMaterial;

            mat = meshRendererBorderLow.material;
        }
        else
        {
            meshRendererImageLow.material = spellardMainBodyMaterial;
            meshRendererBorderLow.material = spellardMainBodyMaterial;
            meshRenderercardBackLow.material = spellardMainBodyMaterial;
            meshRendererIconZoneLow.material = spellardMainBodyMaterial;
            meshRendererNameZoneLow.material = spellardMainBodyMaterial;

            mat = meshRendererBorderLow.material;
        }
        if(cardData.legendary)
        {
            meshRendererImageLow.material = legendaryCardMainBodyMaterial;
            meshRendererBorderLow.material = legendaryCardMainBodyMaterial;
            meshRenderercardBackLow.material = legendaryCardMainBodyMaterial;
            meshRendererIconZoneLow.material = legendaryCardMainBodyMaterial;
            meshRendererNameZoneLow.material = legendaryCardMainBodyMaterial;

            mat = meshRendererBorderLow.material;
        }
        meshRendererImage.material.shader = cardImageShader;

        
    }

    public void SetNotCardInHand()
    {
        cardInHand = false;
    }

    [Button] public void StartAttackCooldown(float duration, bool isSummonCall = false)
    {
        SetNotCardInHand();
        attackOnCD = true;
        doOnce = true;
        currentAttackCoolDown = duration;
        maxAttackCoolDown = duration;
    }

    public void UpdateRPLP(int rp, int lp)
    {
        Debug.Log("Setting rp and lp " + rp + " " + lp);

        if((rp < cardData.rp && lp <= cardData.lp || rp <= cardData.rp && lp < cardData.lp) && (rp > 0 && lp > 0)) 
        {
            cardTakeDamageManager.PlayEffect();
            
        }
        if ((rp < cardData.rp && lp <= cardData.lp || rp <= cardData.rp && lp < cardData.lp))
        {
            SFXLibrary.Instance.hit.PlaySFX();
        }


        cardData.rp = rp;
        cardData.lp = lp;
    }

    public CardData GetData()
    {
        return cardData;
    }

    public void PlayTakeDamageEffect()
    {
        cardTakeDamageManager.PlayEffect();
    }

    public void SetNewDescription(string newDescription)
    {
        cardData.description = newDescription;
    }

    public void PlayBuffEffect()
    {
        buffEffectManager.PlayEffect();
    }
 
    public void SetIntoSpellLimbo()
    {
        timeAsLimboSpell = 0;
        inspellLimbo = true;
    }
    public void RemoveFromSpellLimbo()
    {
        inspellLimbo = false;
    }
    public void SetIntoSummonLimbo()
    {
        timeInSummonLimbo = 0;
        inSummonLimbo = true;
    }
    public void RemoveFromSummonLimbo()
    {
        inSummonLimbo = false;
    }

    public void SetIntoBurnLimbo()
    {
        inBurnLimbo = true;
    }

    public void RemoveFromBurnLimbo()
    {
        inBurnLimbo = false;
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


        } else if (currentAttackCoolDown <= 0 && attackOnCD && !cardInHand) {
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

        if (!References.i.mouse.tutorialMode) { 
            if (unhandled) {
                timeAsUnhandledHandCard += Time.deltaTime;
                if (timeAsUnhandledHandCard > 6) {
                    GameManager.Instance.PlayerReturnDrawCard();
                    unhandled = false;
                }
            }
            if (inSummonLimbo) {
                timeInSummonLimbo += Time.deltaTime;
                if (timeInSummonLimbo > 6) {
                    GameManager.Instance.PlayerReturnCardToHand(gameObject);
                    inSummonLimbo = false;
                }
            }
            if (inBurnLimbo) {
                timeInBurnLimbo += Time.deltaTime;
                if (timeInBurnLimbo > 6) {
                    BurnCardMessage burnCardMessage = new BurnCardMessage();
                    burnCardMessage.denied = true;
                    burnCardMessage.seed = cardData.seed;
                    GameManager.Instance.PlayerBurnCard(burnCardMessage);
                    inBurnLimbo = false;
                }
            }
            if (inspellLimbo) {
                timeAsLimboSpell += Time.deltaTime;
                if (timeAsLimboSpell > 6) {
                    RemoveFromSpellLimbo();
                    GameManager.Instance.ReturnCardToHand(gameObject);
                }
            }

            if (preAttackOnCD && !attackOnCD) {
                timeOnPreAttack += Time.deltaTime;
                if (timeOnPreAttack > 6) {
                    preAttackOnCD = false;
                    ToggleAttackBurnEffect(true);
                    timeOnPreAttack = 0;
                }
            }
        }

    }

    public void MakeCardUnhandled()
    {
        unhandled = true;
    }
    public void UnMakeCardUnhandled()
    {
        unhandled = false;
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
        UnMakeCardUnhandled();
        this.cardData = cardData;
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();
        if(this.cardData.enchantments.Count > 0)
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(true);
            enchantmentCountText.text = this.cardData.enchantments.Count.ToString();
        }
        else
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(false);
        }
        if (cardData.cardSprite) {
            meshRendererImage.material.SetTexture("_CardImage", cardData.cardSprite.texture);
        }

        if (this.cardData.enchantments.Count > 0)
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(true);
            enchantmentCountText.text = this.cardData.enchantments.Count.ToString();
        }
        else
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(false);
        }

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
        SetCardMaterial();

        if (CheckIfSpectral()) spectralEffectManager.StartEffect();
      

    }
    public bool CheckIfSpectral()
    {
        foreach(Enchantment enchantment in cardData.enchantments)
        {
            if(enchantment.enchantmentEffect == Enchantment.EnchantmentEffect.Vanish)
            {
                return true;
            }
        }
        return false;
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
                timeOnPreAttack = 0;
                preAttackOnCD = true;
                ToggleAttackBurnEffect(false);
                if (GameManager.Instance.IsYou(owner)) {
                    if (!References.i.mouse.tutorialMode) {
                        WebSocketService.Attack(cardData.seed);
                    } else {
                        Debug.Log("Attack in tutorial!");
                        CardPowersMessage[] cardPowers = TutorialManager.tutorialManagerInstance.GetAttackTarget(cardData, true);

                        int playerTakenDamage = 0;

                        if (cardData.attackDirection == Card.AttackDirection.Left) {
                            playerTakenDamage = cardData.lp;
                        } else if (cardData.attackDirection == Card.AttackDirection.Right) {
                            playerTakenDamage = cardData.rp;
                        }

                        if (cardPowers.Length == 1) {
                            //direct attack
                            Debug.Log("Player taken dmg " + playerTakenDamage);
                            TutorialManager.tutorialManagerInstance.Attack(true, owner, playerTakenDamage, cardPowers[0]);
                        } else {
                            TutorialManager.tutorialManagerInstance.Attack(false, owner, 0, cardPowers[0], cardPowers[1]);

                        }
                        
                    }
                }
            }
        }
    }



    public void UpdateCardTexts()
    {
        this.lp.text = cardData.lp.ToString();
        this.rp.text = cardData.rp.ToString();
        this.cost.text = cardData.cost.ToString();
        this.value.text = cardData.value.ToString();
    }

    public void Burn()
    {
        ToggleCanAffordEffect(false);
        GetComponent<DissolveEffect>().StartDissolving(mat);
        GetComponent<DissolveMove>().StartDissolving();
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void ReverseBurn()
    {
        GetComponent<DissolveEffect>().StartReverseDissolving(mat);
    }

    [Button] public void SpellBurn()
    {
        ToggleCanAffordEffect(false);
        cardRuneEffectManager.PlayRuneEffect();
    }

    [Button] public void ReverseSpellBurn()
    {
        ToggleCanAffordEffect(true);
        cardRuneEffectManager.PlayReverseRuneEffect();
    }

    public void ToggleGhostCard(bool on)
    {
        if(on)
        {
            isGhostCard = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            textCanvas.enabled = false;
            cardHidden = true;
        }
        else
        {
            isGhostCard = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
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
        if (!References.i.mouse.tutorialMode) {
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
        
    }

    public void StartDestructionEvent()
    {
        interActable = false;
        
        //TODO: play animation here before destroying the card
        StartCoroutine(DestructionAnimation());
    }

    private IEnumerator DestructionAnimation()
    {
        cardRuneEffectManager.PlayRuneDestroyEffect();
        yield return new WaitForSeconds(0.5f);
        SFXLibrary.Instance.cardDestroyed.PlaySFX();
        Destroy(gameObject);
    }

    public void SetDescription()
    {
        descriptionText.text = cardData.description;

        if (this.cardData.enchantments.Count > 0)
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(true);
            enchantmentCountText.text = this.cardData.enchantments.Count.ToString();
        }
        else
        {
            enchantmentDescriptionGameObject.gameObject.SetActive(false);
        }
    }

    public void SetAttackDirectionSymbol()
    {
        if(cardData.attackDirection == Card.AttackDirection.Left)
        {
            Debug.Log("Left");
            if(GameManager.Instance.IsYou(owner))
            {
                Debug.Log("Owner is you");
                leftAttackSymbol.SetActive(true);
                rightDefenceSymbol.SetActive(true);
            }
            else
            {
                Debug.Log("Owner is opponent");
                rigthAttackSymbol.SetActive(true);
                leftDefenceSymbol.SetActive(true);
            }
        }
        else if(cardData.attackDirection == Card.AttackDirection.Right)
        {
            Debug.Log("Right");
            if (GameManager.Instance.IsYou(owner))
            {
                Debug.Log("Owner is you");
                rigthAttackSymbol.SetActive(true);
                leftDefenceSymbol.SetActive(true);
            }
            else
            {
                Debug.Log("Owner is opponent");
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
    public void ToggleTrails(bool state)
    {
        if(state)
        {
            cardTrailManager.EnableTrails();
        }
        else
        {
            cardTrailManager.DisableTrails();
        }
    }

    public void PlayDebuffEffect()
    {
        debuffEffectManager.PlayEffect();
    }

    public void StartDrawCardReadyEffect()
    {
        drawCardReadyManager.StartAnimation();
    }
    public void StopDrawCardReadyEffect()
    {
        drawCardReadyManager.StopAnimation();
    }
    public void RevealDeckCardCountElement()
    {
        deckCardCountElement.SetActive(true);
    }

    public void HideDeckCardCountElement()
    {
        deckCardCountElement.SetActive(false);
    }

    public void StartDrawCooldown(float time)
    {
        drawCDEffectManager.StartDrawCDEffect(time);
    }

    public void RevealCDElement()
    {
        drawCDElement.SetActive(true);
    }

    public void HideCDElement()
    {
        drawCDElement.SetActive(false);
    }

    public void DealDamageToPlayer(int amount)
    {
        dealDamageToPlayerParticleManager.DealDamageToPlayer(amount);
    }
}
