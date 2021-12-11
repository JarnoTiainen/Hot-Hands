using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionBodyScaleManager : MonoBehaviour
{
    [SerializeField] private float defaultScaleX;
    [SerializeField] private float defaultScaleY;

    [SerializeField] private Vector2 defaultResolution;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform rectTransform;

    private Vector2 currentResolution;

    // Start is called before the first frame update
    void Start()
    {
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        currentResolution = rectTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentResolution != (Vector2)canvasRect.sizeDelta)
        {
            Debug.LogError("Changing scale " + currentResolution + " " + (Vector2)canvasRect.sizeDelta);
            float newResolutionX = canvasRect.sizeDelta.x;

            float newFrameXScale = defaultResolution.x / newResolutionX;

            rectTransform.localScale = new Vector2(defaultScaleX * newFrameXScale, defaultScaleY);
            currentResolution = (Vector2)canvasRect.sizeDelta;
        }

    }
}
