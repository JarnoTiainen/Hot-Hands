

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;








public class TutorialManager : MonoBehaviour
{

    public float skipDuration = 3;
    public float attackCoolDown = 3;
    public float spellWindup = 4;
    public Image skipBar;
    public bool burnignAllowed;
    public bool summoningAllowed;
    public bool attackingAllowed;
    public bool firstAttack;
    [SerializeField] private int enemyStartHP;
    [SerializeField] private int startBurnValue = 0;
    [SerializeField] private int enemyStartBurnValue;
    [SerializeField] private float skipTime;
    public List<string> enemyCardSeeds;
    public List<string> spellCardSeed;

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
        SpellCard,
        Damage
    }

    public TutorialState GetState()
    {
        return tutorialState;
    }

    public void NextTutorialState()
    {
        tutorialState++;
    }

    public void SwitchState(TutorialState newState)
    {
        tutorialState = newState;
    }


    private void Awake()
    {
        Debug.Log(GameManager.Instance.attackDuration);
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
    }



    // Update is called once per frame
    void Update()
    {
        if (skipTime >= skipDuration) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.E)) {
            skipTime += Time.deltaTime;
            skipBar.fillAmount = skipTime / skipDuration;
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            skipTime = 0;
            skipBar.fillAmount = 0;
        }

        if (tutorialState == TutorialState.BurnCard && !burnignAllowed) {
            burnignAllowed = true;
            BurnState();
        }

        if (tutorialState == TutorialState.CardPlay && !summoningAllowed) {
            summoningAllowed = true;
        }

        if (tutorialState == TutorialState.CardAttack && !attackingAllowed) {
            attackingAllowed = true;
        }

        

    }

    private void BurnState()
    {
        GameObject burnCard = GameManager.Instance.GetCardFromInGameCards("000000018");
        burnCard.GetComponentsInChildren<HighLightController>()[2].ToggleHighlightAnimation();
    }

    public CardPowersMessage[] GetAttackTarget(CardData data, bool isYourAttack)
    {
        List<GameObject> opponentCards = References.i.opponentMonsterZone.monsterCards;
        List<GameObject> yourCards = References.i.yourMonsterZone.monsterCards;
        GameObject attackTarget = null;

        if (isYourAttack) {
            //direct hit
            if (opponentCards.Count == 0) {
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
        }
    }



}
