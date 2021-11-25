using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardEffectPreview : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;
    [SerializeField] private Shader cardImageShader;
    [SerializeField] private MeshRenderer meshRendererImage;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private float lifeTime;
    [SerializeField] private DescriptionLogoManager descriptionLogoManager;

    [SerializeField] private GameObject rightAttackPower;
    [SerializeField] private GameObject leftAttackPower;

    [SerializeField] private GameObject rightDefencePower;
    [SerializeField] private GameObject leftDefencePower;

    public void SetNewPreviewData(CardData data)
    {
        nameText.text = data.cardName;
        cost.text = data.cost.ToString();
        value.text = data.value.ToString();
        lp.text = data.lp.ToString();
        rp.text = data.rp.ToString();
        meshRendererImage.material.SetTexture("_CardImage", data.cardSprite.texture);

        descriptionLogoManager.SetNewImage(data.enchantments);
        description.text = data.description;


        if(data.attackDirection == Card.AttackDirection.Right)
        {
            rightAttackPower.SetActive(true);
            leftDefencePower.SetActive(true);
        }
        else if(data.attackDirection == Card.AttackDirection.Left)
        {
            leftAttackPower.SetActive(true);
            rightDefencePower.SetActive(true);
        }
        else
        {
            lp.gameObject.SetActive(false);
            rp.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        meshRendererImage.material.shader = cardImageShader;
    }

    private void Update()
    {
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        lifeTime -= Time.deltaTime;
    }
}
