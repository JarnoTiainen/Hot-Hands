using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentEffectGameObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer effectSprite;


    public void StartAnimation(Sprite sprite)
    {
        effectSprite.sprite = sprite;
        Debug.Log("Sprite got now displaying the effect");
    }
}
