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
    public Sprite lastBreathIcon;
    public Sprite openerIcon;
    public Sprite battlecryIcon;
    public Sprite drawtivationIcon;
    public Sprite sacrifice;
    public Sprite retaliate;
    public Sprite brutalityIcon;

    public GameObject fieldCard;
    public GameObject handCard;
    public GameObject opponentDeckObj;
    public GameObject yourDeckObj;
    public GameObject opponentBonfire;
    public GameObject yourBonfire;
    public MonsterZone yourMonsterZone;
    public MonsterZone opponentMonsterZone;
    public AttackEventHandler attackEventHandler;
    public GameObject testCube;

}
