using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SimpleJSON;
using System;

[Serializable][CreateAssetMenu(fileName = "New Card", menuName = "Card/Empty Card")]
public class Card : ScriptableObject
{

    public enum CardType {
        Spell, Monster
    }
    public enum SpellTag
    {
        Trap, Aura, Counter, Quick
    }
    public enum AttackDirection
    {
        Default, Left, Right
    }

    public enum MonsterTag
    {
        Warrior, Dragon
    }

    public bool IsTypeOfCard(CardType type)
    {
        if (type == cardType) return true;
        return false;
    }
    public bool HasTypeOfMonsterTag(MonsterTag monster)
    {
        if (monsterTags.Contains(monster)) return true;
        return false;
    }
    public bool HasTypeOfSpellTag(SpellTag spell)
    {
        if (spellTags.Contains(spell)) return true;
        return false;
    }
    //basic data fields for information all cards share
    [HorizontalGroup("Card Data", 200)] [PreviewField(200)] [HideLabel] public Sprite cardSprite;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] public string cardName;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] public int cost;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] public int value;
    [VerticalGroup("Card Data/Basic Info")] [EnumToggleButtons] [HideLabel] public CardType cardType;

    //info that only spells share
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] [ShowIf("cardType", CardType.Spell)] public List<SpellTag> spellTags;

    //info that only monsters share
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] [ShowIf("cardType", CardType.Monster)] public List<MonsterTag> monsterTags;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] [ShowIf("cardType", CardType.Monster)] public int rp;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] [ShowIf("cardType", CardType.Monster)] public int lp;
    [VerticalGroup("Card Data/Basic Info")] [LabelWidth(100)] [ShowIf("cardType", CardType.Monster)] public AttackDirection attackDirection;

    [SerializeField] public List<Enchantment> enchantments = new List<Enchantment>();


    [Button] public string CreateCardJSON()
    {
        CardJSON cardJSON = new CardJSON(cardName, cost, value, cardType, spellTags, monsterTags, rp, lp, attackDirection, enchantments);
        string cardJSONstring = JsonUtility.ToJson(cardJSON);
        Debug.Log(cardJSONstring);
        return cardJSONstring;
    }

    [Button] public void SaveCardToCardList()
    {
        CardList cardList = Resources.Load<CardList>("Card List");
        cardList.AddAddCardToList(this);
    }
}
