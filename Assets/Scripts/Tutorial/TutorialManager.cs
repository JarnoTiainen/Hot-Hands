using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public float skipDuration = 3;
    public float attackCoolDown = 3;
    public Image skipBar;
    public bool burnignAllowed;
    public bool summoningAllowed;
    public bool attackingAllowed;
    [SerializeField] private int startBurnValue = 0;
    [SerializeField] private float skipTime;

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


    private void Awake()
    {
        tutorialManagerInstance = gameObject.GetComponent<TutorialManager>();
        WebSocketService.Instance.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        skipTime = 0;
        skipBar.fillAmount = 0;
        GameManager.Instance.playerNumber = 0;
        GameManager.Instance.playerStats.playerBurnValue = startBurnValue;
        References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = startBurnValue.ToString();
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E)) {
            skipTime += Time.deltaTime;
            skipBar.fillAmount = skipTime / skipDuration;
        }

        if (skipTime >= skipDuration) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            skipTime = 0;
            skipBar.fillAmount = 0;
        }

        if (tutorialState == TutorialState.BurnCard) {
            burnignAllowed = true;
        }

        if (tutorialState == TutorialState.CardPlay) {
            summoningAllowed = true;
        }

        if (tutorialState == TutorialState.CardAttack) {
            attackingAllowed = true;
        }

    }

    public void SwitchState(TutorialState newState)
    {
        tutorialState = newState;

    }

    public CardPowersMessage[] GetAttackTarget(CardData data)
    {
        List<GameObject> opponentCards = References.i.opponentMonsterZone.monsterCards;
        GameObject attackTarget = null;
        //direct hit
        if (opponentCards.Count == 0) {
            CardPowersMessage cardPowersMessage = new CardPowersMessage(data.seed, data.rp, data.lp);
            CardPowersMessage[] message = {cardPowersMessage};
            return message;
        }

        if (data.attackDirection == Card.AttackDirection.Left) {
            attackTarget = opponentCards[0];
        } else if (data.attackDirection == Card.AttackDirection.Right) {
            attackTarget = opponentCards[-1];
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
}
