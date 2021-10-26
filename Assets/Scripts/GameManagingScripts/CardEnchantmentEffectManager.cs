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

    public void PlayEnchantmentEffect(Enchantment.Trigger trigger)
    {
        Vector3 newEnchantmentEffectPos = transform.position + new Vector3(0, 0, hightFromCard);

        GameObject newEnchantmentEffect = Instantiate(enchantmentEffect, newEnchantmentEffectPos, Quaternion.identity);

        switch(trigger)
        {
            case Enchantment.Trigger.Battlecry:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(battlecrySprite);
                Debug.Log("Batllecry effect here");
                break;
            case Enchantment.Trigger.Brutality:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(brutalitySprite);
                Debug.Log("Brutality effect here");
                break;
            case Enchantment.Trigger.Drawtivation:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(wildSprite);
                Debug.Log("Drawtivation effect here");
                break;
            case Enchantment.Trigger.LastBreath:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(lastBreathSprite);
                Debug.Log("LastBreath effect here");
                break;
            case Enchantment.Trigger.Opener:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(openerSprite);
                Debug.Log("Opener effect here");
                break;
            case Enchantment.Trigger.Retaliate:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(retaliateSprite);
                Debug.Log("Retaliate effect here");
                break;
            case Enchantment.Trigger.Sacrifice:
                newEnchantmentEffect.GetComponent<EnchantmentEffectGameObject>().StartAnimation(sacrificeSprite);
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
