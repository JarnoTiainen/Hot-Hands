using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

public class InGameCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] private CardData cardData;
    [HideInInspector] public int tempRp;
    [HideInInspector] public int tempLp;

    [SerializeField] private bool showReferences;
    [SerializeField] [ShowIf("showReferences", true)] public Canvas textCanvas;
    [SerializeField] [ShowIf("showReferences", true)] public TextMeshProUGUI nameText;
    [SerializeField] [ShowIf("showReferences", true)] private TextMeshProUGUI descriptionText;
    [SerializeField] [ShowIf("showReferences", true)] public TextMeshProUGUI cost;
    [SerializeField] [ShowIf("showReferences", true)] public TextMeshProUGUI value;
    [SerializeField] [ShowIf("showReferences", true)] public TextMeshProUGUI lp;
    [SerializeField] [ShowIf("showReferences", true)] public TextMeshProUGUI rp;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRendererBorderLow;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRenderercardBackLow;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRendererIconZoneLow;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRendererNameZoneLow;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRendererImage;
    [SerializeField] [ShowIf("showReferences", true)] private MeshRenderer meshRendererImageLow;
    [SerializeField] [ShowIf("showReferences", true)] private Slider coolDownSlider;

    [SerializeField] [ShowIf("showReferences", true)] private GameObject leftAttackSymbol;
    [SerializeField] [ShowIf("showReferences", true)] private GameObject rigthAttackSymbol;
    [SerializeField] [ShowIf("showReferences", true)] private GameObject leftDefenceSymbol;
    [SerializeField] [ShowIf("showReferences", true)] private GameObject rightDefenceSymbol;

    [SerializeField] [ShowIf("showReferences", true)] private CardTakeDamageManager cardTakeDamageManager;
    [SerializeField] [ShowIf("showReferences", true)] private SpectralEffectManager spectralEffectManager;
    [SerializeField] [ShowIf("showReferences", true)] private DebuffEffectManager debuffEffectManager;
    [SerializeField] [ShowIf("showReferences", true)] private BuffEffectMaterial buffEffectManager;
    [SerializeField] [ShowIf("showReferences", true)] private CardBurn cardBurn;
    [SerializeField] [ShowIf("showReferences", true)] private CardRuneEffectManager cardRuneEffectManager;
    [SerializeField] [ShowIf("showReferences", true)] private CardTrailManager cardTrailManager;
    [SerializeField] [ShowIf("showReferences", true)] private DrawCardReadyManager drawCardReadyManager;
    [SerializeField] [ShowIf("showReferences", true)] private TextMeshProUGUI enchantmentCountText;
    [SerializeField] [ShowIf("showReferences", true)] private DrawCDEffectManager drawCDEffectManager;
    [SerializeField] [ShowIf("showReferences", true)] private GameObject drawCDElement;
    [ShowIf("showReferences", true)] public GameObject deckCardCountElement;
    [SerializeField] [ShowIf("showReferences", true)] private GameObject enchantmentDescriptionGameObject;

    [SerializeField] [ShowIf("showReferences", true)] private Material cardMainBodyMaterial;
    [SerializeField] [ShowIf("showReferences", true)] private Material spellardMainBodyMaterial;
    [SerializeField] [ShowIf("showReferences", true)] private Material legendaryCardMainBodyMaterial;

    [SerializeField] [ShowIf("showReferences", true)] private Shader cardImageShader;
    [SerializeField] [ShowIf("showReferences", true)] private Material mat;


    public float fadeDuration = 0.5f;
    
    public bool isInHand;
    [HideInInspector] public bool isGhostCard;
    private float maxAttackCoolDown;
    private float currentAttackCoolDown;
    private bool attackOnCD;
    private bool preAttackOnCD;
    private float timeOnPreAttack;
    public int owner;
    private bool doOnce;
    private bool canAffordBool;
    public bool interActable = true;
    public bool cardHidden;

    private float timeAsUnhandledHandCard = 0;
    private bool unhandled;

    private float timeAsLimboSpell = 0;
    private bool inspellLimbo;

    private bool inSummonLimbo;
    private float timeInSummonLimbo;

    private bool inBurnLimbo;
    private float timeInBurnLimbo;

    private bool cardInHand;

    private void Awake()
    {
        SetCardMaterial();
        coolDownSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentAttackCoolDown > 0)
        {
            currentAttackCoolDown -= Time.deltaTime;

            //if the card stopped attacking, show the cooldownbar
            if (!(GetComponent<CardMovement>().doAttack) && (GetComponent<CardMovement>().endPoint.y == transform.localPosition.y))
            {
                if (doOnce)
                {
                    maxAttackCoolDown = currentAttackCoolDown;
                    coolDownSlider.gameObject.SetActive(true);
                    coolDownSlider.GetComponent<CanvasGroup>().alpha = 0;
                    coolDownSlider.value = 1;
                    StartCoroutine(Fade(fadeDuration, true));                   //fade in
                    doOnce = false;
                }
                coolDownSlider.value = currentAttackCoolDown / maxAttackCoolDown;

                //fade out
                if (currentAttackCoolDown <= fadeDuration)
                {
                    StartCoroutine(Fade(fadeDuration, false));
                }

            }


        }
        else if (currentAttackCoolDown <= 0 && attackOnCD && !cardInHand)
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

        if (!References.i.mouse.tutorialMode)
        {
            if (unhandled)
            {
                timeAsUnhandledHandCard += Time.deltaTime;
                if (timeAsUnhandledHandCard > 6)
                {
                    GameManager.Instance.PlayerReturnDrawCard();
                    unhandled = false;
                }
            }
            if (inSummonLimbo)
            {
                timeInSummonLimbo += Time.deltaTime;
                if (timeInSummonLimbo > 6)
                {
                    GameManager.Instance.PlayerReturnCardToHand(gameObject);
                    inSummonLimbo = false;
                }
            }
            if (inBurnLimbo)
            {
                timeInBurnLimbo += Time.deltaTime;
                if (timeInBurnLimbo > 6)
                {
                    BurnCardMessage burnCardMessage = new BurnCardMessage();
                    burnCardMessage.denied = true;
                    burnCardMessage.seed = cardData.seed;
                    GameManager.Instance.PlayerBurnCard(burnCardMessage);
                    inBurnLimbo = false;
                }
            }
            if (inspellLimbo)
            {
                timeAsLimboSpell += Time.deltaTime;
                if (timeAsLimboSpell > 6)
                {
                    RemoveFromSpellLimbo();
                    GameManager.Instance.ReturnCardToHand(gameObject);
                }
            }

            if (preAttackOnCD && !attackOnCD)
            {
                timeOnPreAttack += Time.deltaTime;
                if (timeOnPreAttack > 6)
                {
                    preAttackOnCD = false;
                    ToggleAttackBurnEffect(true);
                    timeOnPreAttack = 0;
                }
            }
        }

    }

    public void SetTempValuesAsValues()
    {
        UpdateRPLP(tempRp, tempLp);
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

    public void OnClickElement()
    {
        if (!Mouse.Instance.targetModeOn && interActable)
        {
            if (!attackOnCD && !preAttackOnCD && GameManager.Instance.IsYou(owner))
            {
                timeOnPreAttack = 0;
                if (!References.i.mouse.tutorialMode) {
                    preAttackOnCD = true;
                }
                
                ToggleAttackBurnEffect(false);
                if (GameManager.Instance.IsYou(owner)) {
                    if (!References.i.mouse.tutorialMode) {
                        WebSocketService.Attack(cardData.seed);
                    } else {
                        if (TutorialManager.tutorialManagerInstance.attackingAllowed) {
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
                        } else {
                            Debug.Log("attacking not allowed");
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
        if (References.i.mouse.tutorialMode) {
            if (cardData.seed == "10000001") {
                TutorialManager.tutorialManagerInstance.NextTutorialState();
            }
        }
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
}
