using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UiCardPreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPreviewGameObject;


    private static List<GameObject> cardPreviews = new List<GameObject>();

    [Button] public void ShowCardPreview(Vector3 pos, Vector3 targetPos = default(Vector3), bool setTargetPosAsStartPos = true)
    {
        Debug.Log(cardPreviewGameObject);
        GameObject newCardPreview = Instantiate(cardPreviewGameObject, pos, Quaternion.identity);
        RectTransform rectTransform = newCardPreview.GetComponent<RectTransform>();
        rectTransform.SetParent(gameObject.transform);
        rectTransform.localPosition = pos;
        newCardPreview.GetComponent<UiCardPreview>().startPos = pos;
        if (setTargetPosAsStartPos) newCardPreview.GetComponent<UiCardPreview>().targetPos = pos;
        else newCardPreview.GetComponent<UiCardPreview>().targetPos = targetPos;
        cardPreviews.Add(newCardPreview);
    }

    [Button] public void HideCardPreview(GameObject cardPreview)
    {
        cardPreviews.Remove(cardPreview);
        Destroy(cardPreview);
    }
}
