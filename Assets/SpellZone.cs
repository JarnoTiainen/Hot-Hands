using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellZone : MonoBehaviour
{
    public static SpellZone Instance { get; private set; }

    private List<GameObject> spells = new List<GameObject>();
    public GameObject spellGameObject;
    public InGameSpell spellSlot1;
    public InGameSpell spellSlot2;
    public InGameSpell spellSlot3;
    private int counter;
    private float timeInTriggerSpellChainLimbo = 0;
    private bool inTriggerSpellChainLimbo = false;

    private void Awake()
    {
        Instance = gameObject.GetComponent<SpellZone>();
    }


    [Button] public void PlaySpell(string seed, List<string> targets, float windup, int slot)
    {
        GameObject newSpell = Instantiate(spellGameObject);
        Debug.Log("slot is " + slot);
        switch(slot)
        {
            case 0:
                spellSlot1.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
                spellSlot1.StartCounter(windup);
                StartCounter(windup);
                foreach (string targetSeed in targets)
                {
                    Debug.Log("Creating Line");
                    spellSlot1.lines.Add(LineRendererManager.Instance.CreateNewLine(spellSlot1.gameObject, GameManager.Instance.GetCardFromInGameCards(targetSeed)));
                }
                break;
            case 1:
                spellSlot2.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
                spellSlot1.StartCounter(windup);
                spellSlot2.StartCounter(windup);
                StartCounter(windup);
                foreach (string targetSeed in targets)
                {
                    Debug.Log("Creating Line");
                    spellSlot2.lines.Add(LineRendererManager.Instance.CreateNewLine(spellSlot2.gameObject, GameManager.Instance.GetCardFromInGameCards(targetSeed)));
                }
                break;
            case 2:
                spellSlot3.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
                spellSlot1.StartCounter(windup);
                spellSlot2.StartCounter(windup);
                spellSlot3.StartCounter(windup);
                StartCounter(windup);
                foreach (string targetSeed in targets)
                {
                    Debug.Log("Creating Line");
                    spellSlot3.lines.Add(LineRendererManager.Instance.CreateNewLine(spellSlot3.gameObject, GameManager.Instance.GetCardFromInGameCards(targetSeed)));
                }
                break;
            default:
                Debug.LogError("Spell overflow from server!");
                break;
        }
        spells.Add(newSpell);
    }

    public void PlaySpell(CardData cardData, List<string> targets, float windup, int slot)
    {
        GameObject newCard = Instantiate(References.i.fieldCard);
        PlaySpell(cardData.seed, targets, windup, slot);
        LimboCardHolder.Instance.StoreNewCard(newCard);
    }
    public void TriggerSpellChain(int index, bool denied)
    {
        switch(index)
        {
            case 0:
                spellSlot1.StartActivateEffect();
                break;
            case 1:
                spellSlot2.StartActivateEffect();
                break;
            case 2:
                spellSlot3.StartActivateEffect();
                break;
        }
    }

    [Button]
    public void StartCounter(float duration)
    {
        CancelInvoke();
        counter = 6;
        int numberOfLights = 6;
        float timeBetweenTicks = duration / numberOfLights;
        InvokeRepeating("Tick", timeBetweenTicks, timeBetweenTicks);
    }

    public void Tick()
    {
        counter--;
        if (counter <= 0.5f && References.i.mouse.tutorialMode) {
            //tutorial stuff
             CancelInvoke();
            StartCoroutine(WaitForDeny());
        } 
        else if (counter <= 0)
        {
            CancelInvoke();
            if (!References.i.mouse.tutorialMode) {
                WebSocketService.TriggerSpellChain();
                timeInTriggerSpellChainLimbo = 0;
                inTriggerSpellChainLimbo = true;
            } 
            else {
                //StatChangeMessage statChangeMessage = new StatChangeMessage();
                TutorialManager.tutorialManagerInstance.TriggerSpellchain();
                SFXLibrary.Instance.spellChainActivation.PlaySFX();
                
            }
        }
        SFXLibrary.Instance.spellChainCountdown.PlaySFX();
    }

    private IEnumerator WaitForDeny()
    {
        //TutorialManager.tutorialManagerInstance.NextTutorialState();
        yield return new WaitUntil(() => TutorialManager.tutorialManagerInstance.spellCardSeed.Count == 3);
        TutorialManager.tutorialManagerInstance.TriggerSpellchain();
        SFXLibrary.Instance.spellChainActivation.PlaySFX();

    }

    private void SpellChainErrorHandler()
    {
        WebSocketService.TriggerSpellChain();
        timeInTriggerSpellChainLimbo = 0;
        inTriggerSpellChainLimbo = true;
    }

    private void Update()
    {
        if(inTriggerSpellChainLimbo)
        {
            timeInTriggerSpellChainLimbo += Time.deltaTime;
            if(timeInTriggerSpellChainLimbo > 6)
            {
                timeInTriggerSpellChainLimbo = 0;
                inTriggerSpellChainLimbo = false;
                SpellChainErrorHandler();
            }
        }
    }

    public bool HasFreeSlot()
    {
        return !spellSlot3.slotTaken;
    }
}
