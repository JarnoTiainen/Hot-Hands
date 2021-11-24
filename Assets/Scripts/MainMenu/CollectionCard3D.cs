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
    [SerializeField] private GameObject healthLeftGO;
    [SerializeField] private GameObject healthRightGO;
    [SerializeField] private GameObject powerLeftGO;
    [SerializeField] private GameObject powerRightGO;

    private void Start()
    {
        nameText.text = card.cardName;
        costText.text = card.cost.ToString();
        rpText.text = card.rp.ToString();
        lpText.text = card.lp.ToString();
        valueText.text = card.value.ToString();
        meshRendererImage.material.SetTexture("_CardImage", card.cardSprite.texture);
        SetAttackDirectionSymbol();
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
        GameObject.FindGameObjectWithTag("DeckBuilder").GetComponent<DeckBuilder>().AddCard(card);
    }
}
