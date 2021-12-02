using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Camera camera;

    private void Start()
    {
        camera = GameObject.Find("CanvasCamera").GetComponent<Camera>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(messageText, Input.mousePosition, camera);

        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = messageText.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
