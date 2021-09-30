using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References: MonoBehaviour
{
    public static References _i;

    public static References i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<References>();
            return _i;
        }
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

}
