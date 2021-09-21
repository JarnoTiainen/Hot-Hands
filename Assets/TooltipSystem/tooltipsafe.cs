using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltipsafe : MonoBehaviour
{
    private static Tooltipsafe instance;
    [SerializeField] private GameObject toolripFramePrefab;
    private Canvas canvas;
    private List<GameObject> toolTipFrames = new List<GameObject>();
    [SerializeField] private float textPaddingSize = 4f;
    [SerializeField] private float gapSizeBetweenTooltips = 0;
    [SerializeField] private float minSizeOfTooltipX = 0;
    [SerializeField] private float minSizeOfTooltipY = 0;
    [SerializeField] private float maxSizeOfTooltipX = 1000;
    [SerializeField] private float maxSizeOfTooltipY = 1000;
    [SerializeField] private float extraPreferredTextSize = 20000;



    private void Awake()
    {
        canvas = transform.parent.gameObject.GetComponent<Canvas>();
        instance = gameObject.GetComponent<Tooltipsafe>();
        HideTooltip();
    }

    private void ShowTooltip(string tooltipString)
    {
        if (toolTipFrames.Count == 0) gameObject.SetActive(true);

        GameObject newToolTipFrame = Instantiate(toolripFramePrefab);
        newToolTipFrame.transform.SetParent(gameObject.transform);

        TextMeshProUGUI tooltipText = newToolTipFrame.transform.Find("TooltipText").gameObject.GetComponent<TextMeshProUGUI>();
        tooltipText.text = tooltipString;

        Vector2 backgroundSize = GetRawTooltipSize(tooltipText);
        RectTransform backgroundRectTransform = newToolTipFrame.transform.Find("Frame").GetComponent<RectTransform>();
        Vector2 backGroundSizeWithBuffer = backgroundSize + new Vector2(textPaddingSize, textPaddingSize);
        backgroundRectTransform.sizeDelta = backGroundSizeWithBuffer;

        newToolTipFrame.GetComponent<RectTransform>().localPosition = GetTooltipPos(newToolTipFrame);







        tooltipText.rectTransform.sizeDelta = backgroundSize;
        tooltipText.rectTransform.localPosition = backgroundRectTransform.localPosition;
        backgroundRectTransform.localPosition -= (Vector3)new Vector2(textPaddingSize / 2, textPaddingSize / 2);

        toolTipFrames.Add(newToolTipFrame);
    }
    private Vector2 GetRawTooltipSize(TextMeshProUGUI tooltipText)
    {
        float backgroundSizeX = tooltipText.preferredWidth + textPaddingSize * 2f;
        if (backgroundSizeX < minSizeOfTooltipX) backgroundSizeX = minSizeOfTooltipX;
        if (backgroundSizeX > maxSizeOfTooltipX) backgroundSizeX = maxSizeOfTooltipX;
        float backgroundSizeY = tooltipText.preferredHeight + textPaddingSize * 2f;
        if (backgroundSizeY < minSizeOfTooltipY) backgroundSizeY = minSizeOfTooltipY;
        if (backgroundSizeY > maxSizeOfTooltipY) backgroundSizeY = maxSizeOfTooltipY;

        Vector2 backgroundSize = new Vector2(backgroundSizeX, backgroundSizeY);
        if (tooltipText.preferredWidth * tooltipText.preferredHeight > backgroundSize.x * backgroundSize.y)
        {
            float newSizeY = (tooltipText.preferredWidth * tooltipText.preferredHeight + extraPreferredTextSize) / backgroundSize.x;
            backgroundSize = new Vector2(backgroundSize.x, newSizeY);
        }
        return backgroundSize;
    }

    private Vector3 GetTooltipPos(GameObject newToolTipFrame)
    {
        float offsetX = newToolTipFrame.GetComponent<RectTransform>().sizeDelta.x / 2;
        float offsetY = newToolTipFrame.GetComponent<RectTransform>().sizeDelta.y / 2;
        Vector3 pos = new Vector3(offsetX, offsetY, 0);

        if (toolTipFrames.Count > 0)
        {
            GameObject previousFrame = toolTipFrames[toolTipFrames.Count - 1];
            float previousFrameYCoordinate = previousFrame.GetComponent<RectTransform>().localPosition.y;
            float previousFrameHight = previousFrame.transform.Find("Frame").gameObject.GetComponent<RectTransform>().sizeDelta.y;
            float newFramePosY = previousFrameYCoordinate + previousFrameHight + gapSizeBetweenTooltips;
            Vector3 newFramePos = newToolTipFrame.GetComponent<RectTransform>().localPosition;
            newFramePos.y = newFramePosY;
            pos = newFramePos;
        }
        return pos;
    }

    private void HideTooltip()
    {
        foreach (GameObject tooltipFrame in toolTipFrames) Destroy(tooltipFrame);
        toolTipFrames = new List<GameObject>();
        gameObject.SetActive(false);
    }
    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos);
        gameObject.transform.position = canvas.transform.TransformPoint(pos);
    }

    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
