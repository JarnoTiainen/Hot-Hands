

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;








public class TutorialManager : MonoBehaviour
{
    public MatchResultScript resultScript;
    public float skipDuration = 3;
    public float attackCoolDown = 3;
    public float spellWindup = 4;
    public Image skipBar;
    public bool drawingAllowed;
    public bool burnignAllowed;
    public bool summoningAllowed;
    public bool attackingAllowed;
    public bool firstAttack;
    public bool firstDirectAttack;
    public bool firstSpell;
    private bool doOnce;
    [SerializeField] private int enemyStartHP;
    [SerializeField] private int startBurnValue = 0;
    [SerializeField] private int enemyStartBurnValue;
    [SerializeField] private float skipTime;
    public List<string> enemyCardSeeds;
    public List<string> spellCardSeed;
    public AIscript opponentAI;
    private DialogueManager diManager;
    private float denycounter;

    [SerializeField] private TutorialState tutorialState = TutorialState.Introduction;
    public static TutorialManager tutorialManagerInstance { get; private set; }

    public enum TutorialState
    {
        Introduction,
        CardDraw,
        Dialogue1,
        BurnCard,
        Dialogue2,
        CardPlay,
        Dialogue3,
        CardAttack,
        Dialogue4,
        AttackValues,
        Dialogue5,
        DefenseValues,
        Dialogue6,
        DirectAttack,
        Dialogue7,
        DummyState,
        Dialogue8,
        SpellCard,
        Dialogue9,
        PlaySpell,
        Dialogue10, //be fast
        Deny,
        Dialogue11, //youy were too 
        Denied,
        Dialogue12, //last dialogue
        End,



    }

    public TutorialState GetState()
    {
        return tutorialState;
    }

    public void NextTutorialState()
    {
        Debug.Log("NEXT" + (int)tutorialState);
        tutorialState++;
        Switcher();      
    }

    public void SwitchState(TutorialState newState)
    {
        tutorialState = newState;
        Debug.Log("switching  state");
        Switcher();
    }

    private void Switcher()
    {
        switch(tutorialState) {
            
            case TutorialState.CardDraw:
                drawingAllowed = true;
                return;
            case TutorialState.Dialogue1:
                ToggleTime();
                diManager.DialogueTrigger();
                drawingAllowed = false;
                return;
            case TutorialState.BurnCard:
                ToggleTime();
                BurnState();
                return;
            case TutorialState.Dialogue2:
                ToggleTime();
                diManager.DialogueTrigger();
                BurnState();
                burnignAllowed = false;
                return;
            case TutorialState.CardPlay:
                ToggleTime();
                PlayCardState();
                return;
            case TutorialState.Dialogue3:
                diManager.DialogueTrigger();
                ToggleTime();
                return;
            case TutorialState.CardAttack:
                attackingAllowed = true;
                ToggleTime();
                AttackState();
                return;
            case TutorialState.Dialogue4:
                diManager.DialogueTrigger();
                return;
            case TutorialState.AttackValues:
                AttackValuesState();
                return;
            case TutorialState.Dialogue5:
                diManager.DialogueTrigger();
                return;
            case TutorialState.DefenseValues:
                AttackValuesState();
                DefenseValuesState();
                return;
            case TutorialState.Dialogue6:
                attackingAllowed = false;
                diManager.DialogueTrigger();
                return;
            case TutorialState.DirectAttack:
                ToggleTime();
                DefenseValuesState();
                StartCoroutine(DirectCounter());
                firstAttack = true;
                return;
            case TutorialState.Dialogue7:
                attackingAllowed = true;
                diManager.DialogueTrigger();
                return;
            case TutorialState.Dialogue8:
                attackingAllowed = false;
                diManager.DialogueTrigger();
                return;
            case TutorialState.SpellCard:
                Debug.Log("Spellcard state");
                drawingAllowed = true;
                opponentAI.OpponentSummonCard();
                return;
            case TutorialState.Dialogue9:
                diManager.DialogueTrigger();
                Debug.Log("Dialogue9");
                ToggleTime();
                return;
            case TutorialState.PlaySpell:
                ToggleTime();
                PlaySpellState();
                return;
            case TutorialState.Dialogue10:
                ToggleTime();
                diManager.DialogueTrigger();
                return;
            case TutorialState.Deny:
                ToggleTime(); //time on
                DenyState();
                return;
            case TutorialState.Dialogue11:
                ToggleTime();
                diManager.DialogueTrigger();
                return;
            case TutorialState.Denied:
                ToggleTime();
                return;
            case TutorialState.Dialogue12:
                ToggleTime();
                diManager.DialogueTrigger();
                return;
            case TutorialState.End:
                summoningAllowed = true;
                attackingAllowed = true;
                burnignAllowed = true;
                drawingAllowed = true;
                ToggleTime();
                return;
            default:
                return;
        }
    }

    private void Awake()
    {
        tutorialManagerInstance = gameObject.GetComponent<TutorialManager>();
        WebSocketService.Instance.enabled = false;
        enemyCardSeeds = new List<string>();
        spellCardSeed = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        skipTime = 0;
        skipBar.fillAmount = 0;
        GameManager.Instance.playerNumber = 0;
        GameManager.Instance.enemyPlayerStats.playerHealth = enemyStartHP;
        GameManager.Instance.playerStats.playerBurnValue = startBurnValue;
        GameManager.Instance.enemyPlayerStats.playerBurnValue = enemyStartBurnValue;
        References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = startBurnValue.ToString();
        diManager = gameObject.GetComponent<DialogueManager>();
    }



    // Update is called once per frame
    void Update()
    {
        if (skipTime >= skipDuration) {
            
            SceneManager.LoadScene(0);
            GameManager.Instance.ResetPlayerStats();
        }

        if (Input.GetKey(KeyCode.E)) {
            skipTime += Time.unscaledDeltaTime;
            skipBar.fillAmount = skipTime / skipDuration;
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            skipTime = 0;
            skipBar.fillAmount = 0;
        }

        if (GameManager.Instance.enemyPlayerStats.playerHealth <= 0) {
            resultScript.GameEnd(true);
        }

        
    }


    private void ToggleTime()
    {
        if(Time.timeScale == 0) {
            Time.timeScale = 1f;
        } else {
            Time.timeScale = 0;
        }
    }

    private void BurnState()
    {
        burnignAllowed = true;
        GameObject burnCard = GameManager.Instance.GetCardFromInGameCards("00000001");
        burnCard.GetComponentsInChildren<HighLightController>()[3].ToggleHighlightAnimation();
        References.i.yourBonfire.GetComponentInChildren<HighLightController>().ToggleHighlightAnimation();
    }

    private void PlayCardState()
    {
        summoningAllowed = true;
        GameObject ownCard = GameManager.Instance.GetCardFromInGameCards("00000000");
        ownCard.GetComponentsInChildren<HighLightController>()[2].ToggleHighlightAnimation();
    }

    private void AttackState()
    {
        attackingAllowed = true;
    }


    private void AttackValuesState()
    {
        GameObject ownCard = GameManager.Instance.GetCardFromInGameCards("00000000");
        GameObject opponentCard = GameManager.Instance.GetCardFromInGameCards("10000000");
        ownCard.GetComponentsInChildren<HighLightController>()[1].ToggleHighlightAnimation();
        opponentCard.GetComponentsInChildren<HighLightController>()[1].ToggleHighlightAnimation();
        if (tutorialState == TutorialState.AttackValues) {
            NextTutorialState();
        }
    }

    private void DefenseValuesState()
    {
        GameObject ownCard = GameManager.Instance.GetCardFromInGameCards("00000000");
        GameObject opponentCard = GameManager.Instance.GetCardFromInGameCards("10000000");
        ownCard.GetComponentsInChildren<HighLightController>()[0].ToggleHighlightAnimation();
        opponentCard.GetComponentsInChildren<HighLightController>()[0].ToggleHighlightAnimation();
        if (tutorialState == TutorialState.DefenseValues) {
            NextTutorialState();
        }
    }

    

    private IEnumerator DirectCounter()
    {
        References.i.opponentDeck.GetComponent<TutorialDeck>().OpponentDraw();
        References.i.opponentDeck.GetComponent<TutorialDeck>().OpponentDraw();

        float countDown = 0;
        while (countDown <= 2) {
            //player attacks by themselves
            //if(firstDirectAttack == true) {
            //    SwitchState(TutorialState.Dialogue8);
            //    break;
            //}
            countDown += Time.deltaTime;
            yield return null;
        }
        //player doesn't attack by themself
        NextTutorialState();
    }

    private void PlaySpellState()
    {
        burnignAllowed = false;
        attackingAllowed = false;
        drawingAllowed = false;
        //does this allow spells
        summoningAllowed = true;
        Debug.Log("Play spell state");
        opponentAI.OpponentPlaySpell();
        GameObject ownCard = GameManager.Instance.GetCardFromInGameCards("00000002");
        ownCard.GetComponentsInChildren<HighLightController>()[1].ToggleHighlightAnimation();
    }

    private void DenyState()
    {
        GameObject ownCard = GameManager.Instance.GetCardFromInGameCards("00000003");
        ownCard.GetComponentsInChildren<HighLightController>()[1].ToggleHighlightAnimation();
        StartCoroutine(DenyNumerator()); 
    }

    private IEnumerator DenyNumerator()
    {
        bool doOnce1 = true;
        while (true) {
            denycounter += Time.deltaTime;
            if (denycounter > 1 && doOnce1) {
                doOnce1 = false;
                ToggleTime();
            }
            if (spellCardSeed.Count == 3) {
                ToggleTime();
                NextTutorialState();
                break;
            }
            yield return null;
        }
        
    }

    public CardPowersMessage[] GetAttackTarget(CardData data, bool isYourAttack)
    {
        List<GameObject> opponentCards = References.i.opponentMonsterZone.monsterCards;
        List<GameObject> yourCards = References.i.yourMonsterZone.monsterCards;
        GameObject attackTarget = null;
        Debug.Log("Opponent field cards outside of if " + opponentCards.Count);
        if (isYourAttack) {
            Debug.Log("IsYourAttack");
            //direct hit
            if (opponentCards.Count == 0) {
                Debug.Log("Opponent field cards " + opponentCards.Count);
                CardPowersMessage cardPowersMessage = new CardPowersMessage(data.seed, data.rp, data.lp);
                CardPowersMessage[] message = { cardPowersMessage };
                return message;
            }

            if (data.attackDirection == Card.AttackDirection.Left) {
                attackTarget = opponentCards[opponentCards.Count - 1];
            } else if (data.attackDirection == Card.AttackDirection.Right) {
                attackTarget = opponentCards[0];
            }
        } else {
            //direct hit
            if (yourCards.Count == 0) {
                CardPowersMessage cardPowersMessage = new CardPowersMessage(data.seed, data.rp, data.lp);
                CardPowersMessage[] message = { cardPowersMessage };
                return message;
            }

            if (data.attackDirection == Card.AttackDirection.Left) {
                attackTarget = yourCards[opponentCards.Count - 1];
            } else if (data.attackDirection == Card.AttackDirection.Right) {
                attackTarget = yourCards[0];
            }
        }

        CardData attackTargetData = attackTarget.GetComponent<InGameCard>().GetCardData();

        int targetNewlp = attackTargetData.lp;
        int targetNewrp = attackTargetData.rp;
        int attackerNewlp = data.lp;
        int attackerNewrp = data.rp;

        if (data.attackDirection == Card.AttackDirection.Left) {
            attackerNewrp = data.rp - attackTargetData.rp;
            targetNewlp = attackTargetData.lp - data.lp;
        } else if (data.attackDirection == Card.AttackDirection.Right) {
            attackerNewlp = data.lp - attackTargetData.lp;
            targetNewrp = attackTargetData.rp - data.rp;
        }

        CardPowersMessage attackerCardPowersMessage = new CardPowersMessage(data.seed, attackerNewrp, attackerNewlp);
        CardPowersMessage targetCardPowersMessage = new CardPowersMessage(attackTargetData.seed, targetNewrp, targetNewlp);
        CardPowersMessage[] messages = { attackerCardPowersMessage, targetCardPowersMessage };

        return messages;
    }

    public void Attack(bool directAttack, int player, int playerTakenDamage, CardPowersMessage attackerPowers, CardPowersMessage targetPowers = null)
    {
        AttackEventMessage attackEventMessage = new AttackEventMessage(false, attackCoolDown, directAttack, playerTakenDamage, player, attackerPowers, targetPowers);

        GameManager.Instance.TutorialPlayerAttack(attackEventMessage);
    }

    public void TriggerSpellchain()
    {
        InvokeRepeating("TriggerSpell", 0, 0.5f);
    }

    public void TriggerSpell()
    {
        string activatingSpell = spellCardSeed[spellCardSeed.Count - 1];
        TriggerSpellMessage triggerSpellMessage = new TriggerSpellMessage(false, spellCardSeed.IndexOf(activatingSpell));
        GameManager.Instance.TriggerSpell(triggerSpellMessage);
        Debug.Log("spellcards count " );
        spellCardSeed.Remove(activatingSpell);
        if (spellCardSeed.Count == 0) {
            Debug.Log("Cancel invoke");
            CancelInvoke();
            GameObject enemyCard = GameManager.Instance.GetCardFromInGameCards("10000001");
            enemyCard.GetComponent<InGameCard>().GetCardData().lp = 0;
            enemyCard.GetComponent<InGameCard>().GetCardData().rp = 0;
            enemyCard.GetComponent<InGameCard>().UpdateCardTexts();
            enemyCard.GetComponent<InGameCard>().StartDestructionEvent();
            References.i.opponentMonsterZone.monsterCards.Remove(enemyCard);
            
             //this goes to 
            //NextTutorialState();
        }
    }

    



}
