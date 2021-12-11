using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlot : MonoBehaviour
{
    public GameObject spellInSlot;

    public void SetNewSpellToslot(GameObject newSpell)
    {
        spellInSlot = newSpell;
    }

    public void RemoveSpellFromSlot()
    {
        spellInSlot = null;
    }

    public GameObject GetSpellInSlot()
    {
        return spellInSlot;
    }
}
