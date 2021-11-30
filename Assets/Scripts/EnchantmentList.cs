using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentList : MonoBehaviour
{
    public static EnchantmentList Instance { get; private set; }


    [SerializeField] [TextArea] private string drawCardDescription;
    [SerializeField] [TextArea] private string gainBurnDescription;
    [SerializeField] [TextArea] private string playSelfDescription;
    [SerializeField] [TextArea] private string discardPileDuplicationDescription;
    [SerializeField] [TextArea] private string summonToEnemyDescription;
    [SerializeField] [TextArea] private string healPlayerDescription;
    [SerializeField] [TextArea] private string buffBoardAttackDirectionPowersDescription;
    [SerializeField] [TextArea] private string essenceSiphonDescription;
    [SerializeField] [TextArea] private string scorchDescription;

    private void Awake()
    {
        Instance = gameObject.GetComponent<EnchantmentList>();
    }

    // :power: is replaced with power.
    public string GetEnchantmentDescription(Enchantment enchantment)
    {
        string effect = "";
        switch (enchantment.enchantmentEffect)
        {
            case Enchantment.EnchantmentEffect.Default:
                effect = "default";
                break;
            case Enchantment.EnchantmentEffect.DrawCard:
                effect = drawCardDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.GainBurn:
                effect = gainBurnDescription.Replace(":power:", AddPlusOrMinus(enchantment.weight));
                break;
            case Enchantment.EnchantmentEffect.PlaySelf:
                effect = playSelfDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.DiscardPileDuplication:
                effect = discardPileDuplicationDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.SummonToEnemy:
                effect = summonToEnemyDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.HealPlayer:
                effect = healPlayerDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.BuffBoardAttackDirectionPowers:
                effect = buffBoardAttackDirectionPowersDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.EssenceSiphon:
                effect = essenceSiphonDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            case Enchantment.EnchantmentEffect.Scorch:
                effect = scorchDescription.Replace(":power:", enchantment.weight.ToString());
                break;
            default:
                effect = "null";
                break;
        }
        switch(enchantment.trigger)
        {
            case Enchantment.Trigger.Opener:
                return "<b>Opener</b>: " + effect;
            case Enchantment.Trigger.Drawtivation:
                return "<b>Wild</b>: " + effect;
            case Enchantment.Trigger.LastBreath:
                return "<b>Lastbreath</b>: " + effect;
            case Enchantment.Trigger.Sacrifice:
                return "<b>Sacrifice</b>: " + effect;
            default:
                return effect;
        }

    }
    public string AddPlusOrMinus(int number)
    {
        if (number > 0)
        {
            return "+ " + number.ToString();
        }
        else if (number < 0)
        {
            return "- " + number.ToString();
        }
        else return number.ToString();
    }
}
