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
    [SerializeField] [TextArea] private string fireballDescription;
    [SerializeField] [TextArea] private string denyDescription;
    [SerializeField] [TextArea] private string destroyCombatantDescription;
    [SerializeField] [TextArea] private string buffSelfPermanentDescription;
    [SerializeField] [TextArea] private string reduceHighestCostDescription;
    [SerializeField] [TextArea] private string damagePlayerDescription;
    [SerializeField] [TextArea] private string vanishDescription;
    [SerializeField] [TextArea] private string addSpellToHanDescription;
    [SerializeField] [TextArea] private string addGolemToDiscardPileDescription;
    [SerializeField] [TextArea] private string buffAllGolemsDescription;
    [SerializeField] [TextArea] private string spellDrawCardDescription;
    [SerializeField] [TextArea] private string stoneCarvingDescription;
    [SerializeField] [TextArea] private string purgatoryDescription;
    [SerializeField] [TextArea] private string silenceDescription;
    [SerializeField] [TextArea] private string armorUpDescription;
    [SerializeField] [TextArea] private string stealTopCardDescription;
    [SerializeField] [TextArea] private string winGameDescription;
    [SerializeField] [TextArea] private string divineDelugeDescription;
    [SerializeField] [TextArea] private string zapDescription;
    [SerializeField] [TextArea] private string blessingOfTheDragonDescription;
    [SerializeField] [TextArea] private string businessDealDescription;
    [SerializeField] [TextArea] private string sharpshotDescription;


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
                effect = drawCardDescription;
                break;
            case Enchantment.EnchantmentEffect.GainBurn:
                effect = gainBurnDescription;
                break;
            case Enchantment.EnchantmentEffect.PlaySelf:
                effect = playSelfDescription;
                break;
            case Enchantment.EnchantmentEffect.DiscardPileDuplication:
                effect = discardPileDuplicationDescription;
                break;
            case Enchantment.EnchantmentEffect.SummonToEnemy:
                effect = summonToEnemyDescription;
                break;
            case Enchantment.EnchantmentEffect.HealPlayer:
                effect = healPlayerDescription;
                break;
            case Enchantment.EnchantmentEffect.BuffBoardAttackDirectionPowers:
                effect = buffBoardAttackDirectionPowersDescription;
                break;
            case Enchantment.EnchantmentEffect.EssenceSiphon:
                effect = essenceSiphonDescription;
                break;
            case Enchantment.EnchantmentEffect.Scorch:
                effect = scorchDescription;
                break;
            case Enchantment.EnchantmentEffect.Fireball:
                effect = fireballDescription;
                break;
            case Enchantment.EnchantmentEffect.Deny:
                effect = denyDescription;
                break;
            case Enchantment.EnchantmentEffect.DestroyCombatant:
                effect = destroyCombatantDescription;
                break;
            case Enchantment.EnchantmentEffect.BuffSelfPermanent:
                effect = buffSelfPermanentDescription;
                break;
            case Enchantment.EnchantmentEffect.ReduceHighestCost:
                effect = reduceHighestCostDescription;
                break;
            case Enchantment.EnchantmentEffect.DamagePlayer:
                effect = damagePlayerDescription;
                break;
            case Enchantment.EnchantmentEffect.Vanish:
                effect = vanishDescription;
                break;
            case Enchantment.EnchantmentEffect.AddSpellToHand:
                effect = addSpellToHanDescription;
                break;
            case Enchantment.EnchantmentEffect.AddGolemToDiscardPile:
                effect = addGolemToDiscardPileDescription;
                break;
            case Enchantment.EnchantmentEffect.BuffAllGolems:
                effect = buffAllGolemsDescription;
                break;
            case Enchantment.EnchantmentEffect.SpellDrawCard:
                effect = spellDrawCardDescription;
                break;
            case Enchantment.EnchantmentEffect.StoneCarving:
                effect = stoneCarvingDescription;
                break;
            case Enchantment.EnchantmentEffect.Purgatory:
                effect = purgatoryDescription;
                break;
            case Enchantment.EnchantmentEffect.Sharphot:
                effect = sharpshotDescription;
                break;
            case Enchantment.EnchantmentEffect.Silence:
                effect = silenceDescription;
                break;
            case Enchantment.EnchantmentEffect.ArmorUp:
                effect = armorUpDescription;
                break;
            case Enchantment.EnchantmentEffect.StealTopCard:
                effect = stealTopCardDescription;
                break;
            case Enchantment.EnchantmentEffect.WinGame:
                effect = winGameDescription;
                break;
            case Enchantment.EnchantmentEffect.DivineDeluge:
                effect = divineDelugeDescription;
                break;
            case Enchantment.EnchantmentEffect.Zap:
                effect = zapDescription;
                break;
            case Enchantment.EnchantmentEffect.BlessingOfTheDragon:
                effect = blessingOfTheDragonDescription;
                break;
            case Enchantment.EnchantmentEffect.BusinessDeal:
                effect = businessDealDescription;
                break;
            default:
                effect = "null";
                break;

                
        }
        effect = effect.Replace(":power:", enchantment.weight.ToString());
        switch (enchantment.trigger)
        {
            case Enchantment.Trigger.Opener:
                return "<b>Opener</b>: " + effect + " ";
            case Enchantment.Trigger.Drawtivation:
                return "<b>Wild</b>: " + effect + " ";
            case Enchantment.Trigger.LastBreath:
                return "<b>Lastbreath</b>: " + effect + " ";
            case Enchantment.Trigger.Sacrifice:
                return "<b>Sacrifice</b>: " + effect + " ";
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
