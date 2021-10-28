using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnchantmentEffectManager : MonoBehaviour
{
    private CardEnchantmentEffectManager gameManager;
    [SerializeField] private GameObject cardEnchantmentEffectPositor;
    public static CardEnchantmentEffectManager Instance { get; private set; }

    public GameObject currentActivePreview;

    [SerializeField] private GameObject enchantmentEffect;
    [SerializeField] private GameObject cardPreview;

    [SerializeField] private float hightFromCard;

    [SerializeField] private Sprite battlecrySprite;
    [SerializeField] private Sprite brutalitySprite;
    [SerializeField] private Sprite wildSprite;
    [SerializeField] private Sprite lastBreathSprite;
    [SerializeField] private Sprite openerSprite;
    [SerializeField] private Sprite retaliateSprite;
    [SerializeField] private Sprite sacrificeSprite;

    public void PlayEnchantmentEffect(Enchantment.Trigger trigger, int index, bool isYou)
    {
        switch (trigger)
        {
            case Enchantment.Trigger.Battlecry:
                
                break;
            case Enchantment.Trigger.Brutality:
                //newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(brutalitySprite);
                Debug.Log("Brutality effect here");
                break;
            case Enchantment.Trigger.Drawtivation:
                if (isYou)
                {
                    Hand.Instance.GetCardWithIndex(index).GetComponent<CardEnchantmentEffectScript>().PlayEffectWild();
                }
                else
                {
                    EnemyHand.Instance.GetCardWithIndex(index).GetComponent<CardEnchantmentEffectScript>().PlayEffectWild();
                }
                Debug.Log("Drawtivation effect here");
                break;
            case Enchantment.Trigger.LastBreath:
                //newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(lastBreathSprite);
                Debug.Log("LastBreath effect here");
                break;
            case Enchantment.Trigger.Opener:
                if (isYou)
                {
                    References.i.yourMonsterZone.GetCardWithServerIndex(index).GetComponent<CardEnchantmentEffectScript>().PlayEffectOpener();

                }
                else
                {
                    References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(index)).GetComponent<CardEnchantmentEffectScript>().PlayEffectOpener();
                }
                Debug.Log("Opener effect here");
                break;
            case Enchantment.Trigger.Retaliate:
                //newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(retaliateSprite);
                Debug.Log("Retaliate effect here");
                break;
            case Enchantment.Trigger.Sacrifice:
                if (isYou)
                {
                    References.i.yourBonfire.GetComponent<CardEnchantmentEffectScript>().PlayEffectSacrifice();
                }
                else
                {
                    References.i.opponentBonfire.GetComponent<CardEnchantmentEffectScript>().PlayEffectSacrifice();
                }
                Debug.Log("Sacrifice effect here");
                break;
        }
    }
    public void DisplayCardEffectSource(CardData data)
    {
        if (currentActivePreview != null) Destroy(currentActivePreview);

        GameObject newCardEffectPreview = Instantiate(cardPreview, cardEnchantmentEffectPositor.transform.position, Quaternion.identity);
        newCardEffectPreview.GetComponent<CardEffectPreview>().SetNewPreviewData(data);
        currentActivePreview = newCardEffectPreview;
    }

}
