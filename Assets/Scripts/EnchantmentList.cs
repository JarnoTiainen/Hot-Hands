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
        switch (enchantment.enchantmentEffect)
        {
            case Enchantment.EnchantmentEffect.Default:
                return "default";
            case Enchantment.EnchantmentEffect.DrawCard:
                return drawCardDescription.Replace(":power:", enchantment.weight.ToString());
            case Enchantment.EnchantmentEffect.GainBurn:
                return gainBurnDescription.Replace(":power:", AddPlusOrMinus(enchantment.weight)); ;
            case Enchantment.EnchantmentEffect.PlaySelf:
                return playSelfDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.DiscardPileDuplication:
                return discardPileDuplicationDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.SummonToEnemy:
                return summonToEnemyDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.HealPlayer:
                return healPlayerDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.BuffBoardAttackDirectionPowers:
                return buffBoardAttackDirectionPowersDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.EssenceSiphon:
                return essenceSiphonDescription.Replace(":power:", enchantment.weight.ToString()); ;
            case Enchantment.EnchantmentEffect.Scorch:
                return scorchDescription.Replace(":power:", enchantment.weight.ToString()); ;
            default:
                return "null";
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
