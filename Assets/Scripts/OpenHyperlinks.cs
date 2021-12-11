using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Camera cameraUI;
    private TMP_LinkInfo linkInfo;

    private void Start()
    {
        cameraUI = GameObject.Find("CanvasCamera").GetComponent<Camera>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(messageText, Input.mousePosition, cameraUI);

        if (linkIndex != -1)  // was a link clicked?
        {
            linkInfo = messageText.textInfo.linkInfo[linkIndex];
            ChatManager.Instance.openLinkConfirmation.SetActive(true);
            ChatManager.Instance.linkConfirmationText.text = "<u>" + gameObject.GetComponent<MessageLinkScript>().link + "</u>";
            ChatManager.Instance.openLinkButton.onClick.AddListener(() => OpenLink());
        }
    }

    public void OpenLink()
    {
        // open the link id as a url, which is the metadata we added in the text field
        ChatManager.Instance.openLinkButton.onClick.RemoveAllListeners();
        ChatManager.Instance.openLinkConfirmation.SetActive(false);
        Application.OpenURL(linkInfo.GetLinkID());
    }
}
