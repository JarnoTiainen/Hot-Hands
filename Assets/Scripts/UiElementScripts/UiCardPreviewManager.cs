
using System.Collections.Generic;
using UnityEngine;

public class UiCardPreviewManager : MonoBehaviour
{
    public static UiCardPreviewManager Instance { get; private set; }

    [SerializeField] private GameObject cardPreviewGameObject;
    private Dictionary<GameObject, GameObject> visibleHandCardPreviews = new Dictionary<GameObject, GameObject>();
    private static List<GameObject> cardPreviews = new List<GameObject>();
    private void Awake()
    {
        Instance = gameObject.GetComponent<UiCardPreviewManager>();
    }

    public GameObject ShowCardPreview(GameObject card)
    {
        GameObject newCardPreview = Instantiate(cardPreviewGameObject);
        newCardPreview.GetComponent<UiCardPreview>().SetNewCardData(card.GetComponent<InGameCard>().GetData());
        RectTransform rectTransform = newCardPreview.GetComponent<RectTransform>();
        rectTransform.SetParent(card.GetComponent<InGameCard>().textCanvas.transform);


        Vector3 cardModelSize = newCardPreview.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().size;
        Vector3 cardPreviewDimensions = newCardPreview.GetComponent<RectTransform>().localScale;
        cardPreviewDimensions = new Vector2(cardModelSize.x * cardPreviewDimensions.x, cardModelSize.z * cardPreviewDimensions.y);

        Vector3 cardDimensions = card.GetComponent<BoxCollider>().size;
        float cardPreviewYpos = 0 - cardDimensions.y / 2 + cardPreviewDimensions.y / 2;

        //if hovered card is on top half of the screen display preview below the card
        if(card.transform.position.y > Camera.main.transform.position.y)
        {
            cardPreviewYpos *= -1;
        }

        rectTransform.localPosition = new Vector3(0, cardPreviewYpos, 0);
        newCardPreview.GetComponent<UiCardPreview>().startPos = new Vector3(0, cardPreviewYpos, 0);
        newCardPreview.GetComponent<UiCardPreview>().targetPos = new Vector3(0, cardPreviewYpos, 0);
        cardPreviews.Add(newCardPreview);

        return newCardPreview;
    }

    public void HideCardPreview(GameObject cardPreview)
    {
        cardPreviews.Remove(cardPreview);
        Destroy(cardPreview);
    }



    //shows card tooltip
    public void ShowCardTooltip(GameObject card)
    {
        visibleHandCardPreviews.Add(card, UiCardPreviewManager.Instance.ShowCardPreview(card));
    }

    //hides card tooltip
    public void HideCardTooltip(GameObject hoveredCard)
    {
        if (visibleHandCardPreviews.ContainsKey(hoveredCard))
        {
            GameObject preview = visibleHandCardPreviews[hoveredCard];
            UiCardPreviewManager.Instance.HideCardPreview(preview);
            visibleHandCardPreviews.Remove(hoveredCard);
        }
    }

}
