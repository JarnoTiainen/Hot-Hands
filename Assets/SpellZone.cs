using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellZone : MonoBehaviour
{
    private List<GameObject> spells = new List<GameObject>();
    public GameObject spellGameObject;
    public InGameSpell spellSlot1;
    public InGameSpell spellSlot2;
    public InGameSpell spellSlot3;


    [Button] public void PlaySpell(string seed)
    {
        GameObject newSpell = Instantiate(spellGameObject);
        if(!spellSlot1.slotTaken)
        {
            spellSlot1.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
        }
        else if(!spellSlot2.slotTaken)
        {
            spellSlot2.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
        }
        else if (!spellSlot3.slotTaken)
        {
            spellSlot3.SetNewSpellToslot(GameManager.Instance.GetCardFromInGameCards(seed));
        }
        else
        {
            Debug.LogError("Spell overflow from server!");
        }
        spells.Add(newSpell);
    }

    public void PlaySpell(CardData cardData)
    {
        GameObject newCard = Instantiate(References.i.fieldCard);
        PlaySpell(cardData.seed);
        LimboCardHolder.Instance.StoreNewCard(newCard);
    }
    public void TriggerSpellChain()
    {

    }
}
