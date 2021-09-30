using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UiCardPreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPreviewGameObject;
    [SerializeField] private float scaleOfPreview;
    Canvas uiCanvas;

    private static List<GameObject> cardPreviews = new List<GameObject>();

    private void Awake()
    {
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
    }

    [Button] public GameObject ShowCardPreview(Vector3 pos, Vector3 targetPos = default(Vector3), bool setTargetPosAsStartPos = true)
    {
        GameObject newCardPreview = Instantiate(cardPreviewGameObject, pos, Quaternion.identity);
        
        RectTransform rectTransform = newCardPreview.GetComponent<RectTransform>();
        Debug.Log(rectTransform.localScale + " " + gameObject.GetComponent<RectTransform>().localScale);
        rectTransform.SetParent(gameObject.GetComponent<RectTransform>());
        rectTransform.localScale = new Vector3(scaleOfPreview, scaleOfPreview, scaleOfPreview);


        Vector3 cardModelSize = newCardPreview.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().size;
        Vector2 cardPreviewDimensions = newCardPreview.GetComponent<RectTransform>().localScale;
        cardPreviewDimensions = new Vector2(cardModelSize.x * cardPreviewDimensions.x, cardModelSize.z * cardPreviewDimensions.y);
        Debug.Log(cardPreviewDimensions);
        Vector2 uiCanvasDimensions = uiCanvas.GetComponent<RectTransform>().sizeDelta;

        if (pos.x > uiCanvasDimensions.x / 2 - cardPreviewDimensions.x / 2)
        {
            pos.x = uiCanvasDimensions.x / 2 - cardPreviewDimensions.x / 2;
        }
        if (pos.x < -uiCanvasDimensions.x / 2 + cardPreviewDimensions.x / 2)
        {
            pos.x = -uiCanvasDimensions.x / 2 + cardPreviewDimensions.x / 2;
        }
        if (pos.y > uiCanvasDimensions.y / 2 - cardPreviewDimensions.y / 2)
        {
            pos.y = uiCanvasDimensions.y / 2 - cardPreviewDimensions.y / 2;
        }
        if (pos.y < -uiCanvasDimensions.y / 2 + cardPreviewDimensions.y / 2)
        {
            pos.y = -uiCanvasDimensions.y / 2 + cardPreviewDimensions.y/2;
        }
        rectTransform.localPosition = pos;
        newCardPreview.GetComponent<UiCardPreview>().startPos = pos;
        if (setTargetPosAsStartPos) newCardPreview.GetComponent<UiCardPreview>().targetPos = pos;
        else newCardPreview.GetComponent<UiCardPreview>().targetPos = targetPos;
        cardPreviews.Add(newCardPreview);

        return newCardPreview;
    }

    [Button] public void HideCardPreview(GameObject cardPreview)
    {
        cardPreviews.Remove(cardPreview);
        Destroy(cardPreview);
    }
}
