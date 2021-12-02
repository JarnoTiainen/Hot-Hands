using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CollectionCard3D : MonoBehaviour
{
    public Card card;
    [SerializeField] private MeshRenderer meshRendererImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI rpText;
    [SerializeField] private TextMeshProUGUI lpText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject healthLeftGO;
    [SerializeField] private GameObject healthRightGO;
    [SerializeField] private GameObject powerLeftGO;
    [SerializeField] private GameObject powerRightGO;
    [SerializeField] private Material cardMainBodyMaterial;
    [SerializeField] private Material spellCardMainBodyMaterial;
    [SerializeField] private Material legendaryCardMainBodyMaterial;
    [SerializeField] private Shader cardImageShader;
    [SerializeField] private MeshRenderer meshRendererBorderLow;
    [SerializeField] private MeshRenderer meshRendererCardBackLow;
    [SerializeField] private MeshRenderer meshRendererIconZoneLow;
    [SerializeField] private MeshRenderer meshRendererNameZoneLow;
    [SerializeField] private MeshRenderer meshRendererImageLow;

    public void Initialize()
    {
        nameText.text = card.cardName;
        costText.text = card.cost.ToString();
        rpText.text = card.rp.ToString();
        lpText.text = card.lp.ToString();
        valueText.text = card.value.ToString();
        string effect = "";
        foreach (Enchantment enchantment in card.enchantments)
        {
            effect += EnchantmentList.Instance.GetEnchantmentDescription(enchantment);
        }
        description.text = effect;
        meshRendererImage.material.SetTexture("_CardImage", card.cardSprite.texture);
        SetAttackDirectionSymbol();
        SetCardMaterial();
    }

    public void SetCardMaterial()
    {
        if (card.cardType == Card.CardType.Monster)
        {
            meshRendererImageLow.material = cardMainBodyMaterial;
            meshRendererBorderLow.material = cardMainBodyMaterial;
            meshRendererCardBackLow.material = cardMainBodyMaterial;
            meshRendererIconZoneLow.material = cardMainBodyMaterial;
            meshRendererNameZoneLow.material = cardMainBodyMaterial;
        }
        else
        {
            meshRendererImageLow.material = spellCardMainBodyMaterial;
            meshRendererBorderLow.material = spellCardMainBodyMaterial;
            meshRendererCardBackLow.material = spellCardMainBodyMaterial;
            meshRendererIconZoneLow.material = spellCardMainBodyMaterial;
            meshRendererNameZoneLow.material = spellCardMainBodyMaterial;
        }
        if (card.legendary)
        {
            meshRendererImageLow.material = legendaryCardMainBodyMaterial;
            meshRendererBorderLow.material = legendaryCardMainBodyMaterial;
            meshRendererCardBackLow.material = legendaryCardMainBodyMaterial;
            meshRendererIconZoneLow.material = legendaryCardMainBodyMaterial;
            meshRendererNameZoneLow.material = legendaryCardMainBodyMaterial;
        }
        meshRendererImage.material.shader = cardImageShader;
    }

    public void SetAttackDirectionSymbol()
    {
        if (card.attackDirection == Card.AttackDirection.Left)
        {
            powerLeftGO.SetActive(true);
            healthRightGO.SetActive(true);
        }
        else if (card.attackDirection == Card.AttackDirection.Right)
        {
            powerRightGO.SetActive(true);
            healthLeftGO.SetActive(true);

        }
        if (card.cardType == Card.CardType.Spell)
        {
            rpText.gameObject.SetActive(false);
            lpText.gameObject.SetActive(false);
        }
    }

    public void AddCard()
    {
        DeckBuilder.Instance.AddCard(card);
    }
}
