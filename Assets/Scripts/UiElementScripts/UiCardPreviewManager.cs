using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UiCardPreviewManager : MonoBehaviour
{
    public static UiCardPreviewManager Instance { get; private set; }

    [SerializeField] private GameObject cardPreviewGameObject;
    [SerializeField] private float scaleOfPreview;
    Canvas uiCanvas;

    private static List<GameObject> cardPreviews = new List<GameObject>();

    private void Awake()
    {
        Instance = gameObject.GetComponent<UiCardPreviewManager>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
    }

    [Button] public GameObject ShowCardPreview(GameObject card)
    {
        GameObject newCardPreview = Instantiate(cardPreviewGameObject);
        newCardPreview.GetComponent<UiCardPreview>().SetNewCardData(card.GetComponent<InGameCard>().cardData);
        RectTransform rectTransform = newCardPreview.GetComponent<RectTransform>();
        rectTransform.SetParent(card.GetComponent<InGameCard>().textCanvas.transform);


        Vector3 cardModelSize = newCardPreview.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().size;
        Vector3 cardPreviewDimensions = newCardPreview.GetComponent<RectTransform>().localScale;
        cardPreviewDimensions = new Vector2(cardModelSize.x * cardPreviewDimensions.x, cardModelSize.z * cardPreviewDimensions.y);

        Vector3 cardDimensions = card.GetComponent<BoxCollider>().size;
        float cardPreviewYpos = 0 - cardDimensions.y / 2 + cardPreviewDimensions.y / 2;

        rectTransform.localPosition = new Vector3(0, cardPreviewYpos, 0);
        newCardPreview.GetComponent<UiCardPreview>().startPos = new Vector3(0, cardPreviewYpos, 0);
        newCardPreview.GetComponent<UiCardPreview>().targetPos = new Vector3(0, cardPreviewYpos, 0);
        cardPreviews.Add(newCardPreview);

        return newCardPreview;
    }

    [Button] public void HideCardPreview(GameObject cardPreview)
    {
        cardPreviews.Remove(cardPreview);
        Destroy(cardPreview);
    }
}
