using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CollectionCard : MonoBehaviour, IOnClickDownUIElement, IOnHoverEnterElement, IOnHoverExitElement
{
    [SerializeField] public CardData cardData;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI value;
    [SerializeField] public TextMeshProUGUI lp;
    [SerializeField] public TextMeshProUGUI rp;

    [SerializeField] private Shader cardMainBodyMaterial;
    [SerializeField] private Material mat;
    [SerializeField] private Texture2D cardImage;
    [SerializeField] private Texture2D lpImage;
    [SerializeField] private Texture2D rpImage;
    [SerializeField] private Texture2D valueImage;
    [SerializeField] private MeshRenderer meshRendererBorderLow;
    [SerializeField] private MeshRenderer meshRenderercardBackLow;
    [SerializeField] private MeshRenderer meshRendererIconZoneLow;
    [SerializeField] private MeshRenderer meshRendererNameZoneLow;
    [SerializeField] private MeshRenderer meshRendererImage;
    [SerializeField] private MeshRenderer meshRendererImageLow;
    [SerializeField] public Canvas textCanvas;





    private void Awake()
    {
        mat = meshRendererBorderLow.material;
        meshRendererImage.material.shader = cardMainBodyMaterial;
        meshRendererImage.material.SetTexture("_sprite", cardImage);
        meshRendererImageLow.material.shader = cardMainBodyMaterial;
        meshRendererBorderLow.material.shader = cardMainBodyMaterial;
        meshRenderercardBackLow.material.shader = cardMainBodyMaterial;
        meshRendererIconZoneLow.material.shader = cardMainBodyMaterial;
        meshRendererNameZoneLow.material.shader = cardMainBodyMaterial;

        /* meshRendererValue.material.renderQueue = 3100;
        meshRendererLP.material.renderQueue = 3100;
        meshRendererRP.material.renderQueue = 3100;
        meshRendererImageLow.material.renderQueue = 3000;
        meshRendererBorderLow.material.renderQueue = 3000;
        meshRenderercardBackLow.material.renderQueue = 3000;
        meshRendererIconZoneLow.material.renderQueue = 2900;
        meshRendererNameZoneLow.material.renderQueue = 2900;
        meshRendererImage.material.renderQueue = 3100; */
    }



    public void SetNewCardData(bool isYourCard, CardData cardData)
    {
        this.cardData = cardData;
        if (name != null) nameText.text = cardData.cardName;
        if (cost != null) cost.text = cardData.cost.ToString();
        if (value != null) value.text = cardData.value.ToString();

        if (isYourCard)
        {
            if (value != null) lp.text = cardData.lp.ToString();
            if (value != null) rp.text = cardData.rp.ToString();
        }
        else
        {
            if (value != null) lp.text = cardData.rp.ToString();
            if (value != null) rp.text = cardData.lp.ToString();
        }

    }

    public void OnHoverEnter()
    {

    }

    public void OnHoverExit()
    {

    }

    public void OnClickElement()
    {

    }

    public void SetStatLp(int lp)
    {
        cardData.lp = lp;
    }

    public void SetStatRp(int rp)
    {
        cardData.rp = rp;
    }

    public void UpdateCardTexts()
    {
        this.lp.text = cardData.lp.ToString();
        this.rp.text = cardData.rp.ToString();
    }

}
