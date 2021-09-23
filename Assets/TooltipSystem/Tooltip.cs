using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;
    [SerializeField] private GameObject toolripFramePrefab;
    private Canvas canvas;
    private List<GameObject> toolTipFrames = new List<GameObject>();
    [SerializeField] private float textPaddingSize;
    [SerializeField] private float gapSizeBetweenTooltips = 0;

    [FoldoutGroup("Tooltip Size Limits")] [SerializeField] private float minSizeOfTooltipX = 0;
    [FoldoutGroup("Tooltip Size Limits")] [SerializeField] private float minSizeOfTooltipY = 0;
    [FoldoutGroup("Tooltip Size Limits")] [SerializeField] private float maxSizeOfTooltipX = 1000;
    [FoldoutGroup("Tooltip Size Limits")] [SerializeField] private float maxSizeOfTooltipY = 1000;
    private float extraPreferredTextSize = 1000;
    [FoldoutGroup("Tooltip Font")][SerializeField] private int tooltipTitleFontSize;
    [FoldoutGroup("Tooltip Font")][SerializeField] private int tooltipTextFontSize;



    private void Awake()
    {
        canvas = transform.parent.gameObject.GetComponent<Canvas>();
        instance = gameObject.GetComponent<Tooltip>();
        HideTooltip();
    }

    private void ShowTooltip(string tooltipString, string title)
    {
        if (toolTipFrames.Count == 0) gameObject.SetActive(true);
        GameObject newToolTipFrame = Instantiate(toolripFramePrefab);
        newToolTipFrame.transform.SetParent(gameObject.transform);

        TextMeshProUGUI tooltipText = newToolTipFrame.transform.Find("TooltipText").gameObject.GetComponent<TextMeshProUGUI>();
        if(tooltipTextFontSize != 0) tooltipText.fontSize = tooltipTextFontSize;

        tooltipText.text = tooltipString;
        if (title != null) newToolTipFrame.transform.Find("Frame").Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        SetRawTooltipSize(newToolTipFrame, title);
        SetTooltipLocation(newToolTipFrame);
        
        RectTransform backgroundRectTransform = newToolTipFrame.transform.Find("Frame").GetComponent<RectTransform>();
        tooltipText.rectTransform.SetParent(backgroundRectTransform);
        tooltipText.rectTransform.localPosition = Vector3.zero;
        backgroundRectTransform.sizeDelta += new Vector2(textPaddingSize * 2, textPaddingSize * 2);
        if (title != null)
        {
            TextMeshProUGUI titleText = newToolTipFrame.transform.Find("Frame").Find("Title").GetComponent<TextMeshProUGUI>();
            if(tooltipTitleFontSize != 0) titleText.fontSize = tooltipTitleFontSize;
            
            tooltipText.rectTransform.localPosition -= new Vector3(0, titleText.fontSize, 0);
            
        }
        

        toolTipFrames.Add(newToolTipFrame);
    }

    private void SetRawTooltipSize(GameObject newToolTipFrame, string title)
    {
        TextMeshProUGUI tooltipText = newToolTipFrame.transform.Find("TooltipText").gameObject.GetComponent<TextMeshProUGUI>();
        //making sure text size parameters are within required values
        float backgroundSizeX = tooltipText.preferredWidth;
        if (backgroundSizeX < minSizeOfTooltipX) backgroundSizeX = minSizeOfTooltipX;
        if (backgroundSizeX > maxSizeOfTooltipX) backgroundSizeX = maxSizeOfTooltipX;
        float backgroundSizeY = tooltipText.preferredHeight;
        if (backgroundSizeY < minSizeOfTooltipY) backgroundSizeY = minSizeOfTooltipY;
        if (backgroundSizeY > maxSizeOfTooltipY) backgroundSizeY = maxSizeOfTooltipY;

        Vector2 backgroundSize = new Vector2(backgroundSizeX, backgroundSizeY);

        //if text preferred square size is smaller than the new tooltip box size, calculate new box hight.
        if ((tooltipText.preferredWidth) * (tooltipText.preferredHeight)  > backgroundSize.x * backgroundSize.y)
        {
            float newSizeY = ((tooltipText.preferredWidth) * (tooltipText.preferredHeight) + extraPreferredTextSize * tooltipText.fontSize) / backgroundSize.x;
            backgroundSize = new Vector2(backgroundSize.x, newSizeY);
        }
        if(title != null)
        {
            TextMeshProUGUI titleText = newToolTipFrame.transform.Find("Frame").Find("Title").GetComponent<TextMeshProUGUI>();
            backgroundSize = new Vector2(backgroundSize.x, backgroundSize.y + titleText.preferredHeight);
        }
        RectTransform backgroundRectTransform = newToolTipFrame.transform.Find("Frame").GetComponent<RectTransform>();
        backgroundRectTransform.sizeDelta = backgroundSize;
        tooltipText.rectTransform.sizeDelta = backgroundSize;
    }

    private void SetTooltipLocation(GameObject newToolTipFrame)
    {
        float offsetX = newToolTipFrame.GetComponent<RectTransform>().sizeDelta.x / 2;
        float offsetY = newToolTipFrame.GetComponent<RectTransform>().sizeDelta.y / 2;
        newToolTipFrame.GetComponent<RectTransform>().localPosition = new Vector3(offsetX, offsetY, 0);

        if (toolTipFrames.Count > 0)
        {
            GameObject previousFrame = toolTipFrames[toolTipFrames.Count - 1];
            float previousFrameYCoordinate = previousFrame.GetComponent<RectTransform>().localPosition.y;
            float previousFrameHight = previousFrame.transform.Find("Frame").gameObject.GetComponent<RectTransform>().sizeDelta.y;
            float newFramePosY = previousFrameYCoordinate + previousFrameHight + gapSizeBetweenTooltips;
            Vector3 newFramePos = newToolTipFrame.GetComponent<RectTransform>().localPosition;
            newFramePos.y = newFramePosY;
            newToolTipFrame.GetComponent<RectTransform>().localPosition = newFramePos;
        }
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

    public static void ShowTooltip_Static(string tooltipString, string titleText = null)
    {
        instance.ShowTooltip(tooltipString, titleText);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
