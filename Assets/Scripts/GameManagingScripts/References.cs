using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References: MonoBehaviour
{
    public static References i { get; private set; }

    private void Awake()
    {
        i = gameObject.GetComponent<References>();
    }

    public GameObject cardBase;
    [SerializeField]public GameObject container;

    public GameObject cardPreviewGameObject;
    //EnchantmentIcons
    public CardList cardList;
    public Texture lastBreathIcon;
    public Texture openerIcon;
    public Texture drawtivationIcon;
    public Texture sacrifice;
    public Color lastBreathColor;
    public Color openerColor;
    public Color drawtivationColor;
    public Color sacrificeColor;


    public GameObject fieldCard;
    public GameObject opponentDeckObj;
    public GameObject yourDeckObj;
    public GameObject opponentBonfire;
    public GameObject monsterzoneContainer;
    public GameObject yourBonfire;
    public MonsterZone yourMonsterZone;
    public MonsterZone opponentMonsterZone;
    public Mouse mouse;
    public AttackEventHandler attackEventHandler;
    public GameObject testCube;
    public GameObject yourPlayerTarget;
    public GameObject enemyPlayerTarget;
    public CardEnchantmentEffectManager cardEnchantmentEffectManager;
    public LifeCounterManager yourLifeCounter;
    public LifeCounterManager opponentLifeCounter;
    public SpellZone spellZone;

    public GameObject yourDeck;
    public GameObject opponentDeck;

    public GameObject yourBurnPile;
   
}
